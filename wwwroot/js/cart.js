function addToCart(plantId) {
    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        data: { id: plantId },
        success: function (response) {
            if (response.success) {
                // Оновлюємо лічильник кошика
                updateCartCounter(response.itemsCount);
                // Показуємо повідомлення про успіх
                showNotification(response.message, 'success');
            } else {
                // Показуємо повідомлення про помилку
                showNotification(response.message, 'error');
                // Якщо користувач не авторизований, перенаправляємо на логін
                if (response.message.includes('авторизуйтесь')) {
                    setTimeout(function () {
                        window.location.href = '/Account/Login';
                    }, 2000);
                }
            }
        },
        error: function () {
            showNotification('Сталася помилка при додаванні товару', 'error');
        }
    });
}

function buyNow(plantId) {
    $.ajax({
        url: '/Cart/BuyNow',
        type: 'POST',
        data: { id: plantId },
        success: function (response) {
            if (response.success) {
                // Перенаправляємо на сторінку оформлення замовлення
                window.location.href = response.redirectUrl;
            } else {
                showNotification(response.message, 'error');
                if (response.message.includes('авторизуйтесь')) {
                    setTimeout(function () {
                        window.location.href = '/Account/Login';
                    }, 2000);
                }
            }
        },
        error: function () {
            showNotification('Сталася помилка при оформленні замовлення', 'error');
        }
    });
}

function updateCartCounter(count) {
    const counter = document.querySelector('.cart-counter');
    if (counter) {
        counter.textContent = count;
    }
}

function showNotification(message, type) {
    // Перевіряємо, чи існує елемент сповіщення
    let notification = document.querySelector('.notification');
    if (!notification) {
        // Якщо не існує, створюємо новий
        notification = document.createElement('div');
        notification.className = 'notification';
        document.body.appendChild(notification);
    }

    // Додаємо клас типу сповіщення
    notification.className = `notification ${type}`;
    notification.textContent = message;

    // Показуємо сповіщення
    notification.style.display = 'block';
    notification.style.opacity = '1';

    // Приховуємо через 3 секунди
    setTimeout(() => {
        notification.style.opacity = '0';
        setTimeout(() => {
            notification.style.display = 'none';
        }, 300);
    }, 3000);
}
