$(document).ready(function () {
    // Initialiser Bootstrap Modal for adoption detaljer/redigering.
    var adoptionModalElement = document.getElementById('adoptionModal');
    var adoptionModal = adoptionModalElement ? new bootstrap.Modal(adoptionModalElement) : null;
    var currentAdoptionId = null; // Holder styr på ID'et for den aktuelle adoption i modalen.

    // --- Global Toast funktion (kan genbruges fra admin-common.js hvis den findes der) ---
    // Sikrer at showToast er tilgængelig. Hvis admin-common.js forventes at definere den,
    // kan denne lokale definition fjernes eller gøres til en fallback.
    if (typeof showToast !== 'function') {
        window.showToast = function(message, type = 'info') {
            var toastId = 'toast-' + new Date().getTime();
            var toastContainer = document.getElementById('toastContainer'); // Forventer en global container
            
            // Opret container hvis den ikke findes (fallback)
            if (!toastContainer) {
                toastContainer = document.createElement('div');
                toastContainer.id = 'toastContainer';
                toastContainer.className = 'position-fixed bottom-0 end-0 p-3';
                toastContainer.style.zIndex = "1056";
                document.body.appendChild(toastContainer);
            }

            var toastHTML = `
                <div id="${toastId}" class="toast align-items-center text-white ${type === 'error' ? 'bg-danger' : (type === 'success' ? 'bg-success' : 'bg-primary')} border-0" role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="7000">
                    <div class="d-flex">
                        <div class="toast-body">
                            ${message}
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>`;
            $(toastContainer).append(toastHTML);
            var toastElement = new bootstrap.Toast(document.getElementById(toastId));
            toastElement.show();
            document.getElementById(toastId).addEventListener('hidden.bs.toast', function () {
                this.remove();
            });
        };
    }

    // Funktion til at indlæse indhold i modalens body.
    function loadModalContent(url, title) {
        if (!adoptionModal) return;
        $('#adoptionModalLabel').text(title);
        $('#adoptionModalBody').html('<div class="text-center py-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Henter...</span></div><p class="mt-2">Henter data...</p></div>');
        $('#adoptionModalFooter').find('#btnSaveAdoptionChanges').remove(); // Fjern evt. tidligere gem-knap
        adoptionModal.show();

        $('#adoptionModalBody').load(url, function (response, status, xhr) {
            if (status === "error") {
                $('#adoptionModalBody').html('<p class="text-danger">Kunne ikke hente indhold: ' + xhr.status + " " + xhr.statusText + '</p>');
            } else {
                // Parse unobtrusive validation for den nye formular, hvis det er en formular der loades.
                var form = $('#adoptionModalBody form');
                if (form.length) {
                    $.validator.unobtrusive.parse(form);
                    // Tilføj Gem-knap til footer hvis det er _AdoptionFormFields
                    if (url.includes("EditAdoptionForm")) { // Eller en anden måde at identificere formen
                        $('#adoptionModalFooter').prepend('<button type="button" class="btn btn-success" id="btnSaveAdoptionChanges"><i class="fas fa-save me-1"></i>Gem ændringer</button>');
                    }
                }
            }
        });
    }

    // Event listener for 'Detaljer' knapper.
    $('#adoptionsTable').on('click', '.btnViewAdoptionDetails', function () {
        currentAdoptionId = $(this).data('adoptionid');
        loadModalContent('?handler=AdoptionDetailsPartial&adoptionId=' + currentAdoptionId, 'Detaljer for Adoption ID: ' + currentAdoptionId);
    });

    // Event listener for 'Rediger' knapper.
    $('#adoptionsTable').on('click', '.btnEditAdoption', function () {
        currentAdoptionId = $(this).data('adoptionid');
        loadModalContent('?handler=EditAdoptionForm&adoptionId=' + currentAdoptionId, 'Rediger Adoption ID: ' + currentAdoptionId);
    });

    // Event listener for dynamisk tilføjet 'Gem ændringer' knap i modalen.
    $(document).on('click', '#btnSaveAdoptionChanges', function() {
        var form = $('#adoptionModalBody form');
        if (!form.length) {
            showToast('Formular ikke fundet i modalen.', 'error');
            return;
        }
        if (!form.valid()) {
            showToast('Formularen indeholder fejl. Ret venligst fejlene og prøv igen.', 'warning');
            return false;
        }

        var formData = form.serialize();

        // Sørg for at currentAdoptionId er sat, hvis det er en redigeringshandling.
        // URL'en kan også bygges mere dynamisk baseret på formens action-attribut, hvis relevant.
        var postUrl = `?handler=EditAdoption&adoptionId=${currentAdoptionId}`;

        $.ajax({
            type: 'POST',
            url: postUrl, // Action URL for redigering (eller oprettelse)
            data: formData,
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // For CSRF beskyttelse
            },
            success: function (response) {
                if (response.success && response.data) {
                    if(adoptionModal) adoptionModal.hide();
                    showToast(response.message || 'Adoption gemt!', 'success');
                    // Opdater tabelrækken dynamisk
                    updateAdoptionRow(response.data);
                } else {
                    // Hvis serveren returnerer HTML (pga. valideringsfejl), genindlæs formularen med fejlene.
                    if (typeof response === 'string') {
                        $('#adoptionModalBody').html(response);
                        var newForm = $('#adoptionModalBody form');
                        if (newForm.length) {
                            $.validator.unobtrusive.parse(newForm);
                        }
                    } else if (response.message) {
                        showToast('Fejl ved gem: ' + response.message, 'error');
                    } else {
                        showToast('Ukendt fejl ved gem af adoption.', 'error');
                    }
                }
            },
            error: function (xhr) {
                var errorMessage = 'Der skete en serverfejl ved gem af adoption.';
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                } else if (xhr.responseText) {
                    // Hvis serveren returnerer HTML (f.eks. en fejlside), kan det vises, men ofte er det bedre med en generisk besked.
                    // Overvej at logge xhr.responseText til konsollen for debugging.
                    console.error("Server error response:", xhr.responseText);
                    $('#adoptionModalBody').html('<div>En serverfejl opstod. Prøv venligst igen. Status: ' + xhr.status + '</div>');
                    // Ingen grund til at returnere her, da fejlen er vist i modalen
                }
                showToast(errorMessage, 'error');
            }
        });
    });

    // Funktion til at generere HTML for status badge.
    function getStatusBadgeHTML(statusValue, statusDisplay) {
        let badgeClass = 'light text-dark'; // Default
        switch (statusValue) { // Antager statusValue er enum-værdien (int eller string)
            case 'Pending': case 0: badgeClass = 'warning text-dark'; break;
            case 'Approved': case 1: badgeClass = 'info text-dark'; break;
            case 'Completed': case 2: badgeClass = 'success'; break;
            case 'Rejected': case 3: badgeClass = 'danger'; break;
            case 'Cancelled': case 4: badgeClass = 'secondary'; break;
        }
        return `<span class="badge bg-${badgeClass}">${statusDisplay}</span>`;
    }
    
    // Funktion til at generere HTML for handlingsknapper.
    function getActionButtonsHTML(adoption) {
        let buttons = 
        `<div class="btn-group btn-group-sm">
            <button type="button" class="btn btn-outline-info btnViewAdoptionDetails" data-adoptionid="${adoption.id}" title="Se detaljer">
                <i class="fas fa-eye"></i>
            </button>
            <button type="button" class="btn btn-outline-primary btnEditAdoption" data-adoptionid="${adoption.id}" title="Rediger adoption">
                <i class="fas fa-edit"></i>
            </button>
        </div>`;
    
        // Status-specifikke knapper
        // Antager adoption.status er enum streng-repræsentation eller numerisk værdi
        if (adoption.status === 'Pending' || adoption.status === 0) {
            buttons += ` <button type="button" class="btn btn-sm btn-outline-success btnApproveAdoption ms-1" data-adoptionid="${adoption.id}" title="Godkend adoption"><i class="fas fa-check"></i></button>
                         <button type="button" class="btn btn-sm btn-outline-warning btnRejectAdoption ms-1" data-adoptionid="${adoption.id}" title="Afvis adoption"><i class="fas fa-times"></i></button>`;
        } else if (adoption.status === 'Approved' || adoption.status === 1) {
            buttons += ` <button type="button" class="btn btn-sm btn-success btnCompleteAdoption ms-1" data-adoptionid="${adoption.id}" title="Marker som gennemført"><i class="fas fa-flag-checkered"></i></button>
                         <button type="button" class="btn btn-sm btn-outline-danger btnCancelAdoption ms-1" data-adoptionid="${adoption.id}" title="Annuller godkendelse"><i class="fas fa-ban"></i></button>`;
        }
        // Tilføj else if for andre statusser (Completed, Rejected, Cancelled) hvis der er handlinger for dem.
        return buttons;
    }

    // Funktion til at opdatere en hel række i tabellen efter redigering.
    function updateAdoptionRow(adoptionData) {
        const row = $(`#adoptionsTable tr[data-adoption-id="${adoptionData.id}"]`);
        if (row.length) {
            row.find('td:nth-child(2)').html(`${adoptionData.animalName || 'N/A'} <small class="text-muted d-block">ID: ${adoptionData.animalId}</small>`);
            row.find('td:nth-child(3)').html(`${adoptionData.customerName || 'N/A'} <small class="text-muted d-block">ID: ${adoptionData.customerId}</small>`);
            row.find('td:nth-child(4)').text(new Date(adoptionData.applicationDate).toLocaleDateString('da-DK'));
            row.find('.adoption-status').html(getStatusBadgeHTML(adoptionData.status, adoptionData.statusDisplay));
            row.find('td:nth-child(6)').text(adoptionData.adoptionDate ? new Date(adoptionData.adoptionDate).toLocaleDateString('da-DK') : '-');
            row.find('.adoption-employee').text(adoptionData.employeeName || 'Ikke tildelt');
            row.find('.adoption-actions').html(getActionButtonsHTML(adoptionData));
        } else {
            // TODO: Håndter tilfælde hvor rækken ikke findes (f.eks. efter oprettelse af ny adoption - kræver tilføjelse af ny række)
            // For nu, reloader vi siden hvis en ny adoption oprettes og rækken ikke findes.
            // Dette er en simplificering; en bedre løsning ville være at tilføje rækken dynamisk.
            // if (isNewAdoption) { // En flag der indikerer om det var en oprettelse
            //    location.reload(); 
            // }
            showToast('Rækken for den opdaterede adoption blev ikke fundet i tabellen. Overvej at genindlæse siden.', 'warning');
        }
    }

    // Funktion til at opdatere status og knapper i en tabelrække efter en statusændring.
    // Denne funktion vil også kalde updateAdoptionRow hvis employeeName skal opdateres.
    function updateAdoptionRowStatusAndButtons(adoptionId, newStatusValue, newStatusDisplay, employeeName, employeeId) {
        const row = $(`#adoptionsTable tr[data-adoption-id="${adoptionId}"]`);
        if (row.length) {
            row.find('.adoption-status').html(getStatusBadgeHTML(newStatusValue, newStatusDisplay));
            row.find('.adoption-employee').text(employeeName || 'Ikke tildelt'); // Opdater medarbejdernavn
            
            // Genopbyg handlingsknapper baseret på den nye fulde adoptionsdata (inkl. det nye status).
            // Vi antager her, at vi har nok info til at bygge knapperne, ellers skal vi hente fuld adoption data.
            // For enkelhedens skyld, simulere en 'adoption' objekt til getActionButtonsHTML.
            var pseudoAdoption = { id: adoptionId, status: newStatusValue }; 
            row.find('.adoption-actions').html(getActionButtonsHTML(pseudoAdoption));
        } else {
            showToast('Kunne ikke finde rækken til at opdatere status.', 'warning');
        }
    }

    // Event listeners for statusændringsknapper.
    $('#adoptionsTable').on('click', '.btnApproveAdoption, .btnRejectAdoption, .btnCompleteAdoption, .btnCancelAdoption', function () {
        var button = $(this);
        var adoptionId = button.data('adoptionid');
        var handlerUrl = '';
        var confirmMessage = 'Er du sikker?';
        // TODO: Hent employeeId fra en global variabel sat ved login, eller fra en skjult input.
        // For nu, bruger vi en hardkodet værdi eller prompter.
        var employeeIdToAssign = parseInt(prompt("Indtast venligst medarbejder ID for denne handling (midlertidig løsning):")) || 0;
        if (employeeIdToAssign === 0 && !button.hasClass('btnCompleteAdoption')) { // Complete behøver ikke nødvendigvis employeeId her
            showToast('Medarbejder ID er påkrævet for denne handling.', 'warning');
            return; 
        }

        if (button.hasClass('btnApproveAdoption')) {
            handlerUrl = `?handler=ApproveAdoption&adoptionId=${adoptionId}&employeeIdToAssign=${employeeIdToAssign}`;
            confirmMessage = 'Er du sikker på, du vil godkende denne adoption?';
        } else if (button.hasClass('btnRejectAdoption')) {
            handlerUrl = `?handler=RejectAdoption&adoptionId=${adoptionId}&employeeIdToAssign=${employeeIdToAssign}`;
            confirmMessage = 'Er du sikker på, du vil afvise denne adoption?';
        } else if (button.hasClass('btnCompleteAdoption')) {
            handlerUrl = `?handler=CompleteAdoption&adoptionId=${adoptionId}`;
            confirmMessage = 'Er du sikker på, du vil markere denne adoption som gennemført?';
        } else if (button.hasClass('btnCancelAdoption')) {
            let reason = prompt("Angiv årsag til annullering (valgfri):");
            if (reason === null) return; // Bruger annullerede prompten.
            handlerUrl = `?handler=CancelAdoption&adoptionId=${adoptionId}&employeeIdToAssign=${employeeIdToAssign}&reason=${encodeURIComponent(reason || 'Annulleret af administrator')}`;
            confirmMessage = 'Er du sikker på, du vil annullere denne adoption/godkendelse?';
        }

        if (!handlerUrl) return;

        if (confirm(confirmMessage)) {
            $.ajax({
                type: 'POST',
                url: handlerUrl,
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.success) {
                        showToast(response.message, 'success');
                        // Serveren bør returnere den opdaterede status (både værdi og displaynavn)
                        // og det nye medarbejdernavn.
                        updateAdoptionRowStatusAndButtons(response.adoptionId, response.newStatusValue, response.newStatusDisplay, response.employeeName, response.employeeId);
                    } else {
                        showToast('Fejl: ' + (response.message || 'Ukendt serverfejl ved statusopdatering.'), 'error');
                    }
                },
                error: function (xhr) {
                    showToast('Serverfejl ved statusopdatering: ' + xhr.status + ' ' + xhr.statusText, 'error');
                }
            });
        }
    });

    // Ryd modal indhold når den lukkes for at undgå cachede data.
    if (adoptionModalElement) {
        adoptionModalElement.addEventListener('hidden.bs.modal', function () {
            $('#adoptionModalBody').html('<div class="text-center py-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Henter...</span></div><p class="mt-2">Henter data...</p></div>');
            $('#adoptionModalFooter').find('#btnSaveAdoptionChanges').remove();
            currentAdoptionId = null;
        });
    }
}); 