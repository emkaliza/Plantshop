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
