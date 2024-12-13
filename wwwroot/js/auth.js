$(document).ready(function () {
    $.validator.unobtrusive.parse(document);

    // Ініціалізація AJAX для форм
    initializeAjaxForms();
});

function initializeAjaxForms() {
    $('form[data-ajax="true"]').each(function () {
        $(this).submit(function (e) {
            e.preventDefault();

            var form = $(this);
            if (!form.valid()) {
                return false;
            }

            $.ajax({
                url: form.attr('action'),
                type: form.attr('data-ajax-method'),
                data: form.serialize(),
                success: function (response) {
                    if (response.success) {
                        if (form.attr('data-ajax-success')) {
                            window[form.attr('data-ajax-success')](response);
                        }
                    } else {
                        showErrors(response.errors);
                    }
                },
                error: function (xhr, status, error) {
                    if (form.attr('data-ajax-failure')) {
                        window[form.attr('data-ajax-failure')](xhr, status, error);
                    }
                }
            });
        });
    });
}

function onLoginSuccess(response) {
    if (response.success) {
        $('#loginModal').modal('hide');
        // Перенаправлення на головну сторінку або оновлення сторінки
        window.location.href = response.redirectUrl || '/';
    } else {
        showErrors(response.errors);
    }
}

function onLoginFailure(xhr, status, error) {
    showErrors(['Сталася помилка під час входу. Будь ласка, спробуйте пізніше.']);
}

function onRegisterSuccess(response) {
    if (response.success) {
        $('#registerModal').modal('hide');
        // Показати повідомлення про успішну реєстрацію
        showSuccess('Реєстрація пройшла успішно! Будь ласка, підтвердіть ваш email.');
    } else {
        showErrors(response.errors);
    }
}

function onRegisterFailure(xhr, status, error) {
    showErrors(['Сталася помилка під час реєстрації. Будь ласка, спробуйте пізніше.']);
}

function showErrors(errors) {
    if (Array.isArray(errors)) {
        errors.forEach(function (error) {
            toastr.error(error);
        });
    }
}

function showSuccess(message) {
    toastr.success(message);
}