$(document).on("click", ".delete", function (e) {
    var self = this;

    bootbox.confirm({
        className: 'modal-primary',
        message: "Are you sure to delete this?",
        buttons: {
            confirm: {
                label: 'Delete',
                className: 'btn-danger'
            },

            cancel: {
                label: 'Close',
                className: 'btn-outline pull-left'
            }

        },

        callback: function (result) {
            if (result) {
                window.location = self.href;
            }
        }
    });

    e.preventDefault();
});