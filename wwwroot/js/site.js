window.onload = function () {
    const rangeInput = document.querySelectorAll(".range-input input");
    const priceInput = document.querySelectorAll(".range-price input");
    const progress = document.querySelector(".range-selected");
    let priceGap = 100;

    priceInput.forEach(input => {
        input.addEventListener("input", e => {
            let minVal = parseInt(priceInput[0].value);
            let maxVal = parseInt(priceInput[1].value);

            if ((maxVal - minVal >= priceGap) && maxVal <= 2000) {
                if (e.target.className === "form-control min-price") {
                    rangeInput[0].value = minVal;
                    progress.style.left = (minVal / rangeInput[0].max) * 100 + "%";
                } else {
                    rangeInput[1].value = maxVal;
                    progress.style.right = 100 - (maxVal / rangeInput[1].max) * 100 + "%";
                }
            }
        });
    });

    rangeInput.forEach(input => {
        input.addEventListener("input", e => {
            let minVal = parseInt(rangeInput[0].value);
            let maxVal = parseInt(rangeInput[1].value);

            if (maxVal - minVal < priceGap) {
                if (e.target.className === "min") {
                    rangeInput[0].value = maxVal - priceGap;
                } else {
                    rangeInput[1].value = minVal + priceGap;
                }
            } else {
                priceInput[0].value = minVal;
                priceInput[1].value = maxVal;
                progress.style.left = (minVal / rangeInput[0].max) * 100 + "%";
                progress.style.right = 100 - (maxVal / rangeInput[1].max) * 100 + "%";
            }
        });
    });
}

function toggleSearch() {
    const searchForm = document.querySelector('.search-form');
    searchForm.classList.toggle('active');

    if (searchForm.classList.contains('active')) {
        document.getElementById('searchInput').focus();
    }
}

document.getElementById('searchInput').addEventListener('keypress', function (e) {
    if (e.key === 'Enter') {
        document.getElementById('searchForm').submit();
    }
});

document.addEventListener('click', function (e) {
    const searchContainer = document.querySelector('.search-container');
    const searchForm = document.querySelector('.search-form');

    if (!searchContainer.contains(e.target) && searchForm.classList.contains('active')) {
        searchForm.classList.remove('active');
    }
});

/*function toggleWishlist(button, plantId) {
    // Визначаємо, чи товар вже в списку бажань
    const isInWishlist = button.querySelector('i').classList.contains('-fill');

    const url = isInWishlist
        ? '/Wishlist/RemoveFromWishlist'
        : '/Wishlist/AddToWishlist';

    $.ajax({
        url: url,
        type: 'POST',
        data: { plantId: plantId },
        success: function (response) {
            if (response.success) {
                // Змінюємо іконку серця
                const icon = button.querySelector('i');
                icon.classList.toggle('');
                icon.classList.toggle('-fill');

                // Можна додати повідомлення про успішну операцію
                *//*toastr.success(isInWishlist
                    ? 'Видалено зі списку бажань'
                    : 'Додано до списку бажань');*//*
            }
        },
        error: function () {
            // Обробка помилок
            toastr.error('Сталася помилка. Спробуйте пізніше.');
        }
    });
}*/

/*function toggleWishlist(button, plantId) {
    const isActive = button.classList.contains('active');
    const url = isActive ? '/Wishlist/RemoveFromWishlist' : '/Wishlist/AddToWishlist';

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ plantId: plantId })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                button.classList.toggle('active');
                const icon = button.querySelector('i');
                icon.classList.toggle('');
                icon.classList.toggle('-fill');
            }
        })
        .catch(error => console.error('Error:', error));
}*/

/*function toggleWishlist(button, plantId) {
    const isActive = button.classList.contains('active');
    const url = isActive ? '/Wishlist/RemoveFromWishlist' : '/Wishlist/AddToWishlist';

    // Додаємо антифоргері токен
    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const formData = new FormData();
    formData.append('plantId', plantId);

    fetch(url, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': tokenElement ? tokenElement.value : '',
        },
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                button.classList.toggle('active');
                const icon = button.querySelector('i');
                icon.classList.toggle('');
                icon.classList.toggle('-fill');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Помилка при оновленні списку бажань');
        });
}*/

/*function toggleWishlist(button, plantId) {
    const isActive = button.classList.contains('active');
    const url = isActive ? '/Wishlist/RemoveFromWishlist' : '/Wishlist/AddToWishlist';

    // Отримуємо токен
    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');

    fetch(url + '?plantId=' + plantId, {  // Змінили спосіб передачі plantId
        method: 'POST',
        headers: {
            'RequestVerificationToken': tokenElement ? tokenElement.value : '',
            'Content-Type': 'application/x-www-form-urlencoded',
        },
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                button.classList.toggle('active');
                const icon = button.querySelector('i');
                icon.classList.toggle('');
                icon.classList.toggle('-fill');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Помилка при оновленні списку бажань');
        });
}
*/

function toggleWishlist(button, plantId) {
    const isActive = button.classList.contains('active');
    const url = isActive ? '/Wishlist/RemoveFromWishlist' : '/Wishlist/AddToWishlist';

    // Отримуємо антифоргері токен
    //const tokenElement = $('input[name="__RequestVerificationToken"]');

    $.ajax({
        url: url,
        type: 'POST',
        data: { plantId: plantId },
        /*headers: {
            'RequestVerificationToken': tokenElement.length ? tokenElement.val() : ''
        },*/
        success: function (response) {
            if (response.success) {
                // Перемикаємо клас active на кнопці
                $(button).toggleClass('active');

                // Оновлюємо іконку
                const icon = $(button).find('i');
                icon.toggleClass('')
                    .toggleClass('-fill');

                // Показуємо повідомлення про успіх
                if (typeof showNotification === 'function') {
                    showNotification('Список бажань оновлено', 'success');
                }
            } else {
                // Показуємо повідомлення про помилку
                if (typeof showNotification === 'function') {
                    showNotification(response.message || 'Помилка при оновленні списку бажань', 'error');
                }
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            if (typeof showNotification === 'function') {
                showNotification('Помилка при оновленні списку бажань', 'error');
            } else {
                alert('Помилка при оновленні списку бажань');
            }
        }
    });
}

$(document).ready(function () {
    // Функція для перемикання табів
    function switchTab(tabId) {
        // Видаляємо активний клас з усіх кнопок
        $('.tab-button').removeClass('active');
        // Додаємо активний клас потрібній кнопці
        $(`.tab-button[data-tab="${tabId}"]`).addClass('active');

        // Ховаємо всі контентні блоки
        $('.tab-pane').removeClass('active');
        // Показуємо потрібний блок
        $(`#${tabId}`).addClass('active');

        // Якщо це вкладка з відгуками, завантажуємо їх
        if (tabId === 'reviews') {
            const plantId = $('#PlantId').val();
            loadReviews(plantId);
        }
    }

    // Обробник кліку по кнопках табів
    $('.tab-button').click(function () {
        const tabId = $(this).data('tab');
        switchTab(tabId);
    });

    // Обробка форми відгуку
    $(document).on('submit', '#reviewForm', function (e) {
        e.preventDefault();
        console.log('Form submitted');

        const formData = {
            plantId: $('#plantId').val(),
            rating: $('input[name="rating"]:checked').val(),
            comment: $('#comment').val()
        };

        // Перевірка наявності рейтингу
        if (!formData.rating) {
            alert('Будь ласка, виберіть рейтинг');
            return;
        }

        $.ajax({
            url: '/Review/AddReview',
            type: 'POST',
            data: formData,
            success: function (response) {
                // Перезавантажуємо відгуки
                loadReviews($('#plantId').val());

                // Очищаємо форму
                $('#reviewForm')[0].reset();

                // Показуємо повідомлення про успіх
                alert('Відгук успішно додано!');
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                alert('Помилка при додаванні відгуку. Спробуйте пізніше.');
            }
        });
    });

    // Функція завантаження відгуків
    function loadReviews(plantId) {
        $.get(`/Review/GetReviews?plantId=${plantId}`, function (data) {
            $('#reviews').html(data);
        });
    }
});