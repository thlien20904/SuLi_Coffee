function searchFood() {
    var input = document.getElementById("searchBox").value.toLowerCase();
    var products = document.getElementsByClassName("product-card");

    for (var i = 0; i < products.length; i++) {
        var title = products[i].getElementsByClassName("product-title")[0].innerText.toLowerCase();
        if (title.includes(input)) {
            products[i].style.display = "";
        } else {
            products[i].style.display = "none";
        }
    }
}

function filterCategory(categoryId) {
    window.location.href = '/SanPham/Info?categoryId=' + categoryId;
}
