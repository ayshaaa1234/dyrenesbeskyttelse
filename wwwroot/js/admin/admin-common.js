// Common admin scripts

// Global Toast Helper Function
function showToast(message, type = 'info') { 
    var toastId = 'toast-' + new Date().getTime();
    var toastBackgroundColor = 'bg-primary'; // Default
    if (type === 'error') toastBackgroundColor = 'bg-danger';
    else if (type === 'success') toastBackgroundColor = 'bg-success';
    else if (type === 'warning') toastBackgroundColor = 'bg-warning';

    var toastHTML = `
        <div id="${toastId}" class="toast align-items-center text-white ${toastBackgroundColor} border-0" role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="5000">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>`;
    
    // Sørg for at toastContainer eksisterer
    let container = document.getElementById('toastContainer');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toastContainer';
        container.className = 'toast-container position-fixed bottom-0 end-0 p-3'; // Standard Bootstrap toast container styling
        container.style.zIndex = '1090'; // Sørg for at den er over de fleste andre elementer
        document.body.appendChild(container);
    }

    $(container).append(toastHTML);
    var toastElement = new bootstrap.Toast(document.getElementById(toastId));
    toastElement.show();
    document.getElementById(toastId).addEventListener('hidden.bs.toast', function () {
        this.remove();
    });
} 