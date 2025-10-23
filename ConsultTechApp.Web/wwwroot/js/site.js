// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const options = {
    duration: 2000,
    position: { x: 'right', y: 'top' },
    dismissible: true,
    types: [
        {
            type: 'info',
            background: '#11cdef',
            icon: {
                className: 'material-icons',
                tagName: 'i',
                text: 'info',
                color: 'white',
            }
        },
        {
            type: 'success',
            background: '#2dce89',
            icon: {
                className: 'material-icons',
                tagName: 'i',
                text: 'check_circle',
                color: 'white',
            }
        },
        {
            type: 'warning',
            background: '#fb6340',
            icon: {
                className: 'material-icons',
                tagName: 'i',
                text: 'warning',
                color: 'white',
            }
        },
        {
            type: 'danger',
            background: '#f5365c',
            icon: {
                className: 'material-icons',
                tagName: 'i',
                text: 'dangerous',
                color: 'white',
            }
        }
    ]
}

const notyf = new Notyf(options);

// 🧩 Interface utilitaire
const notyfInterface = {
    info: (message) => notyf.open({ type: 'info', message }),
    success: (message) => notyf.open({ type: 'success', message }),
    warning: (message) => notyf.open({ type: 'warning', message }),
    danger: (message) => notyf.open({ type: 'danger', message }),
    show: (type, message) => {
        if (!type || !message) return;
        if (type === 'info') notyfInterface.info(message);
        else if (type === 'success') notyfInterface.success(message);
        else if (type === 'warning') notyfInterface.warning(message);
        else if (type === 'danger') notyfInterface.danger(message);
    }
};

// ================================
// Message depuis TempData
// ================================
window.addEventListener('load', () => {
    const messageArea = document.querySelector('#notyf-area');
    if (!messageArea) return;

    const message = messageArea.textContent.trim();
    if (!message) return;

    if (messageArea.classList.contains('error'))
        notyfInterface.danger(message);
    else if (messageArea.classList.contains('success'))
        notyfInterface.success(message);
    else
        notyfInterface.info(message);
});