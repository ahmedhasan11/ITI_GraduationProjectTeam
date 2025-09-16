// ==================== ON DOCUMENT READY ====================
document.addEventListener('DOMContentLoaded', function () {
    // ==================== Navbar Scroll Effect ====================
    const navbar = document.querySelector('.navbar');
    const navbarCollapse = document.getElementById('navbarContent');
    const closeMobileBtn = document.querySelector('[data-close-mobile-menu]');

    window.addEventListener('scroll', function () {
        if (window.scrollY > 10) {
            navbar.classList.add('navbar-scrolled');
        } else {
            navbar.classList.remove('navbar-scrolled');
        }
    });

        document.addEventListener("DOMContentLoaded", function () {
        const slides = document.querySelectorAll(".slide");
        let currentSlide = 0;

        function showSlide(index) {
            slides.forEach((slide, i) => {
                slide.classList.toggle("active", i === index);
            });
        }

        function nextSlide() {
            currentSlide = (currentSlide + 1) % slides.length;
        showSlide(currentSlide);
        }

        // Show the first slide initially
        showSlide(currentSlide);

        // Change slide every 5 seconds
        setInterval(nextSlide, 5000);
    });
    // ==================== Custom Dropdown Toggle (for non-bootstrap) ====================
    document.querySelectorAll('.nav-item.dropdown > a').forEach(item => {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            const dropdownMenu = this.nextElementSibling;

            // Close all other dropdowns
            document.querySelectorAll('.dropdown-menu').forEach(menu => {
                if (menu !== dropdownMenu) menu.classList.remove('show');
            });

            // Toggle current dropdown
            dropdownMenu.classList.toggle('show');
        });
    });

    // ==================== Close Dropdowns when clicking outside ====================
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.nav-item.dropdown')) {
            document.querySelectorAll('.dropdown-menu').forEach(menu => menu.classList.remove('show'));
        }
    });

    // ==================== Close Mobile Menu Button ====================
    if (closeMobileBtn) {
        closeMobileBtn.addEventListener('click', function () {
            navbarCollapse.classList.remove('show');
        });
    }

    // ==================== Smooth Scrolling for Anchor Links ====================
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();

            const targetId = this.getAttribute('href');
            if (targetId === '#') return;

            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                window.scrollTo({
                    top: targetElement.offsetTop - 80,
                    behavior: 'smooth'
                });
            }
        });
    });

    // ==================== Slider Functionality ====================
    const slides = document.querySelectorAll('.slide');
    const dots = document.querySelectorAll('.slider-dot');
    let currentSlide = 0;

    function showSlide(n) {
        slides.forEach(slide => slide.classList.remove('active'));
        dots.forEach(dot => dot.classList.remove('active'));

        currentSlide = (n + slides.length) % slides.length;

        slides[currentSlide].classList.add('active');
        dots[currentSlide].classList.add('active');
    }

    if (slides.length > 0) {
        setInterval(() => showSlide(currentSlide + 1), 5000);

        dots.forEach((dot, index) => {
            dot.addEventListener('click', () => showSlide(index));
        });
    }

    // ==================== Wishlist Toggle ====================
    document.querySelectorAll('.wishlist-btn').forEach(button => {
        button.addEventListener('click', function () {
            const icon = this.querySelector('i');
            if (icon.classList.contains('far')) {
                icon.classList.replace('far', 'fas');
                this.style.color = '#ff6b6b';
            } else {
                icon.classList.replace('fas', 'far');
                this.style.color = '#64748b';
            }
        });
    });

    // ==================== Add-to-Cart Animation + Backend ====================
    document.querySelectorAll('.add-to-cart-btn').forEach(button => {
        button.addEventListener('click', function () {
            const medicineId = this.dataset.medicineId;
            addToCart(medicineId); // Call backend function

            // Animation
            this.innerHTML = '<i class="fas fa-check"></i> Added to Cart';
            this.style.background = 'linear-gradient(135deg, #28a745, #20c997)';
            setTimeout(() => {
                this.innerHTML = '<i class="fas fa-cart-plus"></i> Add to Cart';
                this.style.background = '';
            }, 2000);
        });
    });

    // ==================== Cart Dropdown Click (Load Items) ====================
    const cartDropdown = document.getElementById("cartDropdown");
    const cartItemsContainer = document.getElementById("cart-dropdown-items");

    if (cartDropdown) {
        cartDropdown.addEventListener("click", function () {
            fetch("/Cart/GetCartItemsDropdown")
                .then(res => res.text())
                .then(html => {
                    cartItemsContainer.innerHTML = html;
                })
                .catch(() => {
                    cartItemsContainer.innerHTML = "<p class='text-danger px-3'>Error loading cart</p>";
                });
        });
    }

    // ==================== Initial Cart Refresh ====================
    refreshCart();
});

// ==================== REFRESH CART FUNCTION ====================
async function refreshCart() {
    try {
        const countRes = await fetch('/Cart/GetCartCount');
        const countData = await countRes.json();
        document.getElementById('cart-count').innerText = countData.count;

        const dropdownRes = await fetch('/Cart/GetCartItemsDropdown');
        const dropdownHtml = await dropdownRes.text();
        document.getElementById('cart-dropdown-items').innerHTML = dropdownHtml;
    } catch {
        document.getElementById('cart-dropdown-items').innerHTML =
            "<p class='text-danger px-3'>Error loading cart</p>";
    }
}

// ==================== ADD TO CART FUNCTION ====================
async function addToCart(medicineId, quantity = 1) {
    const res = await fetch('/Cart/Add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ MedicineId: medicineId, Quantity: quantity })
    });

    const data = await res.json();
    if (data.success) {
        await refreshCart();
    } else {
        alert("Failed to add to cart");
    }
}
