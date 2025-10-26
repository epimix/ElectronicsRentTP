// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function configConfirmModal(dialogId, actionUrl) {
    var modal = document.querySelector('#' + dialogId);

    modal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget;
        var id = button.getAttribute('data-bs-id');
        var title = button.getAttribute('data-bs-title');

        var actionBtn = modal.querySelector('.actionBtn');
        actionBtn.href = actionUrl + id;

        var titleElement = modal.querySelector('.title');
        if (titleElement && title) {
            titleElement.textContent = title;
        }
    });
}
