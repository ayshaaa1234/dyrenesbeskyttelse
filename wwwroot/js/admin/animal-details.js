$(document).ready(function () {
    // AnimalId skal initialiseres fra siden, der inkluderer dette script.
    // F.eks. ved at have en <script>var initialAnimalId = @Model.Animal.Id;</script> før dette script inkluderes,
    // eller ved at læse det fra et data-attribut på et kendt element.
    // For nu antager vi, at 'animalId' er en global variabel, der er sat af Razor-siden.
    // Det er bedre at passere det som parameter til funktioner eller læse fra data-attributter.

    if (typeof animalId === 'undefined') {
        console.error("animalId er ikke defineret. Sørg for at sætte denne variabel på siden, der inkluderer animal-details.js");
        // For at undgå yderligere fejl, stopper vi udførelsen af resten af scriptet her.
        // Du kan evt. tilføje en mere brugervenlig fejlhåndtering.
        // return;
    }

    // --- showToast er nu global og forventes inkluderet via admin-common.js ---

    // --- HealthRecord Modal Logic ---
    var healthRecordModalElement = document.getElementById('healthRecordModal');
    var healthRecordModal = healthRecordModalElement ? new bootstrap.Modal(healthRecordModalElement) : null;
    var currentHealthRecordId = null;

    function formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        return `${day}-${month}-${year}`;
    }

    function buildHealthRecordRow(record, currentAnimalId) {
        return `
            <tr data-record-id="${record.id}">
                <td>${formatDate(record.recordDate)}</td>
                <td>${record.veterinarianName || ''}</td>
                <td>${record.diagnosis || ''}</td>
                <td>${record.treatment || ''}</td>
                <td>${record.notes || ''}</td>
                <td>${record.isVaccinated ? 'Ja' : 'Nej'}</td>
                <td>${formatDate(record.nextVaccinationDate)}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-outline-primary btnEditHealthRecord" data-recordid="${record.id}" data-animalid="${currentAnimalId}">Rediger</button>
                    <button type="button" class="btn btn-sm btn-outline-danger btnDeleteHealthRecord" data-recordid="${record.id}" data-animalid="${currentAnimalId}" data-recordname="Notat fra ${formatDate(record.recordDate)}">Slet</button>
                </td>
            </tr>`;
    }

    function updateHealthRecordsTable(newRecord, isEdit = false, currentAnimalId) {
        const tbody = $('#healthRecordsTableContainer tbody');
        const noRecordsP = $('#healthRecordsTableContainer p');
        const table = $('#healthRecordsTableContainer table');

        if (isEdit) {
            tbody.find(`tr[data-record-id="${newRecord.id}"]`).replaceWith(buildHealthRecordRow(newRecord, currentAnimalId));
        } else {
            tbody.prepend(buildHealthRecordRow(newRecord, currentAnimalId));
        }

        var rows = tbody.find('tr').get();
        rows.sort(function (a, b) {
            var dateAStr = $(a).find('td:first').text(); 
            var dateBStr = $(b).find('td:first').text();
            var dateA = new Date(dateAStr.split('-').reverse().join('-'));
            var dateB = new Date(dateBStr.split('-').reverse().join('-'));
            return dateB - dateA; // Descending
        });
        $.each(rows, function (index, row) {
            tbody.append(row);
        });

        const currentCount = parseInt($('#healthRecordsCount').text());
        if (!isEdit) {
            $('#healthRecordsCount').text(currentCount + 1);
        }

        if (tbody.find('tr').length > 0) {
            noRecordsP.hide();
            table.show();
        } else {
            noRecordsP.show();
            table.hide();
            $('#healthRecordsCount').text(0);
        }
    }

    $('#btnAddHealthRecord').click(function () {
        const localAnimalId = $(this).data('animalid'); // Hent animalId fra knappen
        currentHealthRecordId = null;
        $('#healthRecordModalLabel').text('Tilføj Sundhedsnotat');
        $('#healthRecordModalBody').load('?handler=CreateHealthRecordForm&animalId=' + localAnimalId, function () {
            $.validator.unobtrusive.parse($('#healthRecordModalBody form'));
            if(healthRecordModal) healthRecordModal.show();
        });
    });

    $(document).on('click', '.btnEditHealthRecord', function () {
        currentHealthRecordId = $(this).data('recordid');
        const localAnimalId = $(this).data('animalid'); 
        $('#healthRecordModalLabel').text('Rediger Sundhedsnotat');
        $('#healthRecordModalBody').load('?handler=EditHealthRecordForm&recordId=' + currentHealthRecordId + '&animalId=' + localAnimalId, function () {
            $.validator.unobtrusive.parse($('#healthRecordModalBody form'));
            if(healthRecordModal) healthRecordModal.show();
        });
    });

    $('#btnSaveHealthRecord').click(function () {
        var form = $('#healthRecordModalBody form');
        if (!form.length) {
            showToast('Formular ikke fundet.', 'error');
            return;
        }
        if (!form.valid()) return false;

        // Hent animalId fra formens action eller et skjult felt, hvis det er mere robust
        // Her antager vi, at den er korrekt i URL'en der bygges.
        const formAction = form.attr('action') || '';
        let localAnimalId = new URLSearchParams(formAction.split('?')[1]).get('animalId');
        if (!localAnimalId && currentHealthRecordId) { // Hvis det er edit, er animalId måske ikke i action, find den fra hidden field hvis muligt
            localAnimalId = form.find('input[name="HealthRecord.AnimalId"]').val(); 
        }
        if (!localAnimalId && animalId) { // Fallback til global animalId, hvis den er sat
            localAnimalId = animalId;
        } 
        if (!localAnimalId) {
             showToast('Animal ID mangler for at gemme sundhedsnotat.', 'error');
             return;
        }

        var formData = form.serialize();
        var url = currentHealthRecordId ?
            `?handler=EditHealthRecord&recordId=${currentHealthRecordId}&animalId=${localAnimalId}` :
            `?handler=CreateHealthRecord&animalId=${localAnimalId}`;

        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success && response.data) {
                    if(healthRecordModal) healthRecordModal.hide();
                    showToast(response.message, 'success');
                    updateHealthRecordsTable(response.data, !!currentHealthRecordId, localAnimalId);
                } else {
                    if (typeof response === 'string') {
                        $('#healthRecordModalBody').html(response);
                        $.validator.unobtrusive.parse($('#healthRecordModalBody form'));
                    } else if (response.message) {
                        showToast('Fejl: ' + response.message, 'error');
                    }
                }
            },
            error: function (xhr) {
                var errorMessage = 'Der skete en serverfejl ved gem af sundhedsnotat.';
                if (xhr.responseJSON && xhr.responseJSON.message) errorMessage = xhr.responseJSON.message;
                else if (xhr.responseText) {
                    $('#healthRecordModalBody').html('<div>En serverfejl opstod. Prøv venligst igen. Status: ' + xhr.status + '</div>');
                    return;
                }
                showToast(errorMessage, 'error');
            }
        });
    });

    $('#healthRecordsTableContainer').on('click', '.btnDeleteHealthRecord', function () {
        var recordIdToDelete = $(this).data('recordid');
        var localAnimalIdHrDel = $(this).data('animalid');
        var recordName = $(this).data('recordname') || 'dette notat';
        if (confirm(`Er du sikker på, at du vil slette ${recordName}?`)) {
            $.ajax({
                url: `?handler=DeleteHealthRecord&recordId=${recordIdToDelete}&animalId=${localAnimalIdHrDel}`,
                type: 'POST',
                headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
                success: function (response) {
                    if (response.success) {
                        $('#healthRecordsTableContainer tbody tr[data-record-id="' + recordIdToDelete + '"]').remove();
                        const countSpan = $('#healthRecordsCount');
                        const currentCount = parseInt(countSpan.text());
                        const newCount = Math.max(0, currentCount - 1);
                        countSpan.text(newCount);
                        if (newCount === 0) {
                            $('#healthRecordsTableContainer table').hide();
                            $('#healthRecordsTableContainer p').show();
                        }
                        showToast(response.message || 'Sundhedsnotat slettet!', 'success');
                    } else {
                        showToast('Fejl under sletning: ' + (response.message || 'Ukendt fejl.'), 'error');
                    }
                },
                error: function (xhr) {
                    var errorMessage = 'Der skete en serverfejl under sletning.';
                    if (xhr.responseJSON && xhr.responseJSON.message) errorMessage = xhr.responseJSON.message;
                    showToast(errorMessage, 'error');
                }
            });
        }
    });

    if (healthRecordModalElement) {
        healthRecordModalElement.addEventListener('hidden.bs.modal', function () {
            $('#healthRecordModalBody').html('');
            currentHealthRecordId = null;
        });
    }

    // --- Visit Modal Logic ---
    var visitModalElement = document.getElementById('visitModal');
    var visitModal = visitModalElement ? new bootstrap.Modal(visitModalElement) : null;
    var currentVisitId = null;
    // Enum værdier for VisitStatus skal være tilgængelige for JavaScript
    // Dette kan gøres ved at definere dem i en global JS variabel i Razor viewet
    // eller ved at sende dem med i AJAX responses hvis nødvendigt.
    // For nu antager vi at visitStatusEnum er et objekt som { Scheduled: 'Scheduled', Confirmed: 'Confirmed', ...}
    // som er defineret i Razor-siden.
    if (typeof visitStatusEnum === 'undefined') {
        console.warn("visitStatusEnum er ikke defineret. Handlingsknapper for besøg vil måske ikke virke korrekt.");
        // Definer en tom fallback for at undgå fejl, men funktionalitet vil være begrænset.
        visitStatusEnum = { Scheduled: 'Scheduled', Confirmed: 'Confirmed', Completed: 'Completed', Cancelled: 'Cancelled' }; 
    }


    function formatVisitDateTime(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        const hours = String(date.getHours()).padStart(2, '0');
        const minutes = String(date.getMinutes()).padStart(2, '0');
        return `${day}-${month}-${year} ${hours}:${minutes}`;
    }

    function getVisitActionButtons(statusToString, visitId, currentAnimalId, plannedDate) {
        let visitNameDate = plannedDate ? formatVisitDateTime(plannedDate) : "dette besøg";
        let buttons = `<button type="button" class="btn btn-sm btn-outline-primary btnEditVisit" data-visitid="${visitId}" data-animalid="${currentAnimalId}">Rediger</button>
                       <button type="button" class="btn btn-sm btn-outline-danger btnDeleteVisit" data-visitid="${visitId}" data-animalid="${currentAnimalId}" data-visitname="Besøg d. ${visitNameDate}">Slet</button>`;

        if (statusToString === visitStatusEnum.Scheduled) {
            buttons += ` <button type="button" class="btn btn-sm btn-outline-success btnConfirmVisit ms-1" data-visitid="${visitId}">Bekræft</button>
                         <button type="button" class="btn btn-sm btn-outline-warning btnCancelVisit ms-1" data-visitid="${visitId}">Annuller</button>`;
        } else if (statusToString === visitStatusEnum.Confirmed) {
            buttons += ` <button type="button" class="btn btn-sm btn-success btnCompleteVisit ms-1" data-visitid="${visitId}">Fuldfør</button>
                         <button type="button" class="btn btn-sm btn-outline-warning btnCancelVisit ms-1" data-visitid="${visitId}">Annuller</button>`;
        }
        return buttons;
    }
    
    function buildVisitRow(visit, currentAnimalId) {
        return `
            <tr data-visit-id="${visit.id}">
                <td>${formatVisitDateTime(visit.plannedDate)}</td>
                <td>${visit.plannedDuration} min.</td>
                <td>${visit.type || ''}</td>
                <td>${visit.purpose || ''}</td>
                <td>${visit.visitor || ''}</td>
                <td class="visit-status">${visit.statusDisplay}</td> 
                <td>${visit.notes || ''}</td>
                <td class="visit-actions">
                    ${getVisitActionButtons(visit.status.toString(), visit.id, currentAnimalId, visit.plannedDate)}
                </td>
            </tr>`;
    }

    function updateVisitsTable(newVisit, isEdit = false, currentAnimalId) {
        const tbody = $('#visitsTableContainer tbody');
        const noRecordsP = $('#visitsTableContainer p');
        const table = $('#visitsTableContainer table');

        if (isEdit) {
            tbody.find(`tr[data-visit-id="${newVisit.id}"]`).replaceWith(buildVisitRow(newVisit, currentAnimalId));
        } else {
            tbody.prepend(buildVisitRow(newVisit, currentAnimalId));
        }

        var rows = tbody.find('tr').get();
        rows.sort(function (a, b) {
            var dateAStr = $(a).find('td:first').text();
            var dateBStr = $(b).find('td:first').text();
            var isoDateA = dateAStr.substring(6, 10) + '-' + dateAStr.substring(3, 5) + '-' + dateAStr.substring(0, 2) + 'T' + dateAStr.substring(11, 16);
            var isoDateB = dateBStr.substring(6, 10) + '-' + dateBStr.substring(3, 5) + '-' + dateBStr.substring(0, 2) + 'T' + dateBStr.substring(11, 16);
            var dateA = new Date(isoDateA);
            var dateB = new Date(isoDateB);
            return dateB - dateA; // Descending
        });
        $.each(rows, function (index, row) {
            tbody.append(row);
        });

        const currentCount = parseInt($('#visitsCount').text());
        if (!isEdit) {
            $('#visitsCount').text(currentCount + 1);
        }

        if (tbody.find('tr').length > 0) {
            noRecordsP.hide();
            table.show();
        } else {
            noRecordsP.show();
            table.hide();
            $('#visitsCount').text(0);
        }
    }

    $('#btnAddVisit').click(function () {
        const localAnimalId = $(this).data('animalid'); 
        currentVisitId = null;
        $('#visitModalLabel').text('Registrer Besøg');
        $('#visitModalBody').load('?handler=CreateVisitForm&animalId=' + localAnimalId, function () {
            $.validator.unobtrusive.parse($('#visitModalBody form'));
            if(visitModal) visitModal.show();
        });
    });

    $(document).on('click', '.btnEditVisit', function () {
        currentVisitId = $(this).data('visitid');
        const localAnimalId = $(this).data('animalid');
        $('#visitModalLabel').text('Rediger Besøg');
        $('#visitModalBody').load('?handler=EditVisitForm&visitId=' + currentVisitId + '&animalId=' + localAnimalId, function () {
            $.validator.unobtrusive.parse($('#visitModalBody form'));
            if(visitModal) visitModal.show();
        });
    });

    $('#btnSaveVisit').click(function () {
        var form = $('#visitModalBody form');
        if (!form.length) {
            showToast('Formular ikke fundet.', 'error');
            return;
        }
        if (!form.valid()) return false;
        
        const formAction = form.attr('action') || '';
        let localAnimalId = new URLSearchParams(formAction.split('?')[1]).get('animalId');
        if (!localAnimalId && currentVisitId) { 
            localAnimalId = form.find('input[name="Visit.AnimalId"]').val(); 
        }
        if (!localAnimalId && animalId) { 
            localAnimalId = animalId;
        } 
        if (!localAnimalId) {
             showToast('Animal ID mangler for at gemme besøg.', 'error');
             return;
        }

        var formData = form.serialize();
        var url = currentVisitId ?
            `?handler=EditVisit&visitId=${currentVisitId}&animalId=${localAnimalId}` :
            `?handler=CreateVisit&animalId=${localAnimalId}`;

        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success && response.data) {
                    if(visitModal) visitModal.hide();
                    showToast(response.message, 'success');
                    updateVisitsTable(response.data, !!currentVisitId, localAnimalId);
                } else {
                    if (typeof response === 'string') {
                        $('#visitModalBody').html(response); // Antager form er wrapped i en form tag.
                        $.validator.unobtrusive.parse($('#visitModalBody form'));
                    } else if (response.message) {
                        showToast('Fejl: ' + response.message, 'error');
                    }
                }
            },
            error: function (xhr) {
                var errorMessage = 'Der skete en serverfejl ved gem af besøg.';
                if (xhr.responseJSON && xhr.responseJSON.message) errorMessage = xhr.responseJSON.message;
                else if (xhr.responseText) {
                    $('#visitModalBody').html('<div>En serverfejl opstod. Prøv venligst igen. Status: ' + xhr.status + '</div>');
                    return;
                }
                showToast(errorMessage, 'error');
            }
        });
    });

    $('#visitsTableContainer').on('click', '.btnDeleteVisit', function () {
        var visitIdToDelete = $(this).data('visitid');
        var localAnimalIdVisitDel = $(this).data('animalid');
        var visitName = $(this).data('visitname') || 'dette besøg';
        if (confirm(`Er du sikker på, at du vil slette ${visitName}?`)) {
            $.ajax({
                url: `?handler=DeleteVisit&visitId=${visitIdToDelete}&animalId=${localAnimalIdVisitDel}`,
                type: 'POST',
                headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
                success: function (response) {
                    if (response.success) {
                        $('#visitsTableContainer tbody tr[data-visit-id="' + visitIdToDelete + '"]').remove();
                        const countSpan = $('#visitsCount');
                        const currentCount = parseInt(countSpan.text());
                        const newCount = Math.max(0, currentCount - 1);
                        countSpan.text(newCount);
                        if (newCount === 0) {
                            $('#visitsTableContainer table').hide();
                            $('#visitsTableContainer p').show();
                        }
                        showToast(response.message || 'Besøg slettet!', 'success');
                    } else {
                        showToast('Fejl under sletning af besøg: ' + (response.message || 'Ukendt fejl.'), 'error');
                    }
                },
                error: function (xhr) {
                    var errorMessage = 'Der skete en serverfejl under sletning af besøg.';
                    if (xhr.responseJSON && xhr.responseJSON.message) errorMessage = xhr.responseJSON.message;
                    showToast(errorMessage, 'error');
                }
            });
        }
    });

    if(visitModalElement) {
        visitModalElement.addEventListener('hidden.bs.modal', function () {
            $('#visitModalBody').html('');
            currentVisitId = null;
        });
    }

    // --- Visit Status Change Logic ---
    function handleVisitStatusUpdate(visitId, actionUrl, currentAnimalId) {
        if (!confirm(`Er du sikker på, at du vil ændre status for dette besøg?`)) {
            return;
        }

        $.ajax({
            type: 'POST',
            url: actionUrl, // URL indeholder allerede visitId og animalId fra knappen
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success && response.data) {
                    showToast(response.message, 'success');
                    const visitRow = $(`#visitsTableContainer tr[data-visit-id="${response.data.id}"]`);
                    if (visitRow.length) {
                        visitRow.find('.visit-status').text(response.data.statusDisplay);
                        visitRow.find('.visit-actions').html(getVisitActionButtons(response.data.status.toString(), response.data.id, currentAnimalId, response.data.plannedDate));
                    } else {
                        console.warn("Kunne ikke finde besøgsrækken til opdatering.");
                    }
                } else {
                    showToast(response.message || 'Fejl ved opdatering af besøgsstatus.', 'error');
                }
            },
            error: function (xhr) {
                var errorMessage = 'Serverfejl ved opdatering af besøgsstatus.';
                if (xhr.responseJSON && xhr.responseJSON.message) errorMessage = xhr.responseJSON.message;
                showToast(errorMessage, 'error');
            }
        });
    }

    $(document).on('click', '.btnConfirmVisit, .btnCancelVisit, .btnCompleteVisit', function () {
        var visitId = $(this).data('visitid');
        // animalId skal hentes fra et sted, hvor det er pålideligt. 
        // Hvis knapperne er inde i en række med data-animalid, kan det bruges.
        // Eller hvis 'animalId' global variabel er sat korrekt.
        var localAnimalId = $(this).closest('tr').find('.btnEditVisit').data('animalid'); // Prøv at finde via edit knap
        if (!localAnimalId) localAnimalId = animalId; // Fallback til global

        var handlerName = '';
        if ($(this).hasClass('btnConfirmVisit')) handlerName = 'ConfirmVisit';
        else if ($(this).hasClass('btnCancelVisit')) handlerName = 'CancelVisit';
        else if ($(this).hasClass('btnCompleteVisit')) handlerName = 'CompleteVisit';
        else return; // Ukendt knap

        if (!localAnimalId) {
            showToast('Animal ID kunne ikke findes for statusopdatering.', 'error');
            return;
        }

        var actionUrl = `?handler=${handlerName}&visitId=${visitId}&animalId=${localAnimalId}`;
        handleVisitStatusUpdate(visitId, actionUrl, localAnimalId);
    });
}); 