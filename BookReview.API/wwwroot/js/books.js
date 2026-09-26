let currentPage = 1;
let currentSearch = "";

const pageSize = 6;


// CATEGORY FILTERS

let selectedCategories = [];


// LOAD CATEGORIES


async function loadCategories() {

    const container =
        document.getElementById("categoriesContainer");

    if (!container) {
        return;
    }

    try {

        const response =
            await apiRequest(
                "/Categories?page=1&pageSize=100"
            );

        if (!response.ok) {

            container.innerHTML = `
                <div class="text-danger">
                    Nije moguće učitati kategorije.
                </div>
            `;

            return;
        }

        const data =
            await response.json();

        if (!data.items || data.items.length === 0) {

            container.innerHTML = `
                <div class="text-muted">
                    Nema dostupnih kategorija.
                </div>
            `;

            return;
        }

        container.innerHTML =
            data.items.map(category => `

                <div class="col-md-3 mb-2">

                    <div class="form-check">

                        <input
                            class="form-check-input category-checkbox"
                            type="checkbox"
                            value="${category.id}"
                            id="category-${category.id}"
                        >

                        <label
                            class="form-check-label"
                            for="category-${category.id}"
                        >
                            ${escapeHtml(category.name)}
                        </label>

                    </div>

                </div>

            `).join("");


        document
            .querySelectorAll(".category-checkbox")
            .forEach(checkbox => {

                checkbox.addEventListener(
                    "change",
                    function () {

                        selectedCategories =
                            Array.from(
                                document.querySelectorAll(
                                    ".category-checkbox:checked"
                                )
                            ).map(x => Number(x.value));

                        loadBooks(1);
                    }
                );

            });

    } catch {

        container.innerHTML = `
            <div class="text-danger">
                Greška prilikom učitavanja kategorija.
            </div>
        `;
    }
}


// CLEAR CATEGORY FILTERS

function clearCategoryFilters() {

    selectedCategories = [];

    document
        .querySelectorAll(".category-checkbox")
        .forEach(checkbox => {
            checkbox.checked = false;
        });

    loadBooks(1);
}


// BOOK LIST


async function loadBooks(page = 1) {

    const container =
        document.getElementById("booksContainer");

    if (!container) {
        return;
    }

    currentPage = page;

    container.innerHTML = `
        <div class="text-center py-5">

            <div class="spinner-border"></div>

            <p class="mt-2">
                Učitavanje knjiga...
            </p>

        </div>
    `;

    try {

        let url =
            `/Books?page=${page}&pageSize=${pageSize}`;

        // SEARCH
        if (currentSearch) {

            url +=
                `&search=${encodeURIComponent(currentSearch)}`;
        }

        // CATEGORIES
        if (selectedCategories.length > 0) {

            url +=
                `&categoryIds=${selectedCategories.join(",")}`;
        }

        const response =
            await apiRequest(url);

        if (!response.ok) {

            container.innerHTML = `
                <div class="alert alert-danger">
                    Nije moguće učitati knjige.
                </div>
            `;

            return;
        }

        const data =
            await response.json();

        if (!data.items || data.items.length === 0) {

            container.innerHTML = `
                <div class="alert alert-info">
                    Nema pronađenih knjiga.
                </div>
            `;

            renderPagination(0);

            return;
        }

        container.innerHTML =
            data.items.map(book => {

                const cover =
                    book.coverImagePath
                        ? book.coverImagePath
                        : "https://via.placeholder.com/300x400?text=No+Cover";

                // AUTHORS
                const authors =
                    book.authors &&
                        book.authors.length > 0

                        ? book.authors
                            .map(author =>
                                `${escapeHtml(author.firstName)}
                                 ${escapeHtml(author.lastName)}`
                            )
                            .join(", ")

                        : "Nepoznat autor";

                // CATEGORIES
                const categories =
                    book.categories &&
                        book.categories.length > 0

                        ? book.categories
                            .map(category => `
                                <span class="badge bg-secondary me-1">
                                    ${escapeHtml(category.name)}
                                </span>
                            `)
                            .join("")

                        : `<span class="text-muted">
                            Bez kategorije
                           </span>`;

                return `

                    <div class="col-md-4 mb-4">

                        <div class="card book-card shadow-sm h-100">

                            <img
                                src="${cover}"
                                class="card-img-top"
                                alt="${escapeHtml(book.title)}"
                            >

                            <div class="card-body d-flex flex-column">

                                <h5 class="card-title">
                                    ${escapeHtml(book.title)}
                                </h5>

                                <p class="mb-2">

                                    <strong>
                                        Autor:
                                    </strong>

                                    ${authors}

                                </p>

                                <div class="mb-2">

                                    ${categories}

                                </div>

                                <p class="text-muted mb-2">

                                    ISBN:
                                    ${escapeHtml(book.isbn)}

                                </p>

                                <p class="card-text">

                                    ${escapeHtml(
                    (book.description || "")
                        .substring(0, 120)
                )}

                                    ${book.description &&
                        book.description.length > 120
                        ? "..."
                        : ""
                    }

                                </p>

                                <p class="text-muted">

                                    Godina:
                                    ${book.publishedYear}

                                </p>

                                <a
                                    href="/book-details.html?id=${book.id}"
                                    class="btn btn-primary mt-auto"
                                >
                                    Detalji
                                </a>

                            </div>

                        </div>

                    </div>

                `;

            }).join("");

        renderPagination(data.totalPages);

    } catch {

        container.innerHTML = `
            <div class="alert alert-danger">
                Došlo je do greške.
            </div>
        `;
    }
}


// PAGINATION

function renderPagination(totalPages) {

    const pagination =
        document.getElementById("pagination");

    if (!pagination) {
        return;
    }

    if (totalPages <= 1) {

        pagination.innerHTML = "";

        return;
    }

    let html = `
        <nav>
            <ul class="pagination justify-content-center">
    `;

    for (
        let i = 1;
        i <= totalPages;
        i++
    ) {

        html += `

            <li class="page-item
                ${i === currentPage ? "active" : ""}">

                <a
                    class="page-link"
                    href="#"
                    onclick="loadBooks(${i}); return false;"
                >
                    ${i}
                </a>

            </li>

        `;
    }

    html += `
            </ul>
        </nav>
    `;

    pagination.innerHTML = html;
}

// BOOK DETAILS

async function loadBookDetails() {

    const container =
        document.getElementById("bookDetails");

    if (!container) {
        return;
    }

    const params =
        new URLSearchParams(
            window.location.search
        );

    const id =
        params.get("id");

    if (!id) {

        container.innerHTML = `
            <div class="alert alert-danger">
                Knjiga nije pronađena.
            </div>
        `;

        return;
    }

    try {

        const response =
            await apiRequest(
                `/Books/${id}`
            );

        if (!response.ok) {

            container.innerHTML = `
                <div class="alert alert-danger">
                    Knjiga nije pronađena.
                </div>
            `;

            return;
        }

        const book =
            await response.json();

        const cover =
            book.coverImagePath
                ? book.coverImagePath
                : "https://via.placeholder.com/300x400?text=No+Cover";

        // AUTHORS
        const authors =
            book.authors &&
                book.authors.length > 0

                ? book.authors
                    .map(author => `
                        <div>
                            <strong>
                                ${escapeHtml(author.firstName)}
                                ${escapeHtml(author.lastName)}
                            </strong>
                        </div>
                    `)
                    .join("")

                : `<span class="text-muted">
                    Nepoznat autor
                   </span>`;

        // CATEGORIES
        const categories =
            book.categories &&
                book.categories.length > 0

                ? book.categories
                    .map(category => `
                        <span class="badge bg-secondary me-1">
                            ${escapeHtml(category.name)}
                        </span>
                    `)
                    .join("")

                : `<span class="text-muted">
                    Bez kategorije
                   </span>`;

        container.innerHTML = `

            <div class="row">

                <!-- COVER -->

                <div class="col-md-4">

                    <img
                        src="${cover}"
                        class="img-fluid rounded shadow"
                        alt="${escapeHtml(book.title)}"
                    >

                </div>

                <!-- DETAILS -->

                <div class="col-md-8">

                    <h1 class="mb-3">
                        ${escapeHtml(book.title)}
                    </h1>

                    <div class="mb-3">

                        <strong>
                            Autor:
                        </strong>

                        <div class="mt-1">
                            ${authors}
                        </div>

                    </div>

                    <div class="mb-3">

                        <strong>
                            Kategorije:
                        </strong>

                        <div class="mt-1">
                            ${categories}
                        </div>

                    </div>

                    <p class="text-muted">

                        <strong>
                            ISBN:
                        </strong>

                        ${escapeHtml(book.isbn)}

                    </p>

                    <p>

                        <strong>
                            Godina izdanja:
                        </strong>

                        ${book.publishedYear}

                    </p>

                    <p class="mt-4">

                        ${escapeHtml(
            book.description || ""
        )}

                    </p>

                    <!-- FAVORITE -->

                    <button
                        id="favoriteButton"
                        class="btn btn-outline-danger mt-3"
                    >
                        ♡ Dodaj u favorite
                    </button>

                </div>

            </div>

        `;

        await checkFavorite(
            book.id
        );

        await loadReviews(
            book.id
        );

    } catch {

        container.innerHTML = `
            <div class="alert alert-danger">
                Greška prilikom učitavanja knjige.
            </div>
        `;
    }
}


// FAVORITES

async function checkFavorite(bookId) {

    const button =
        document.getElementById(
            "favoriteButton"
        );

    if (!button) {
        return;
    }

    if (!isLoggedIn()) {

        button.innerHTML =
            "♡ Dodaj u favorite";

        button.classList.remove(
            "btn-danger"
        );

        button.classList.add(
            "btn-outline-danger"
        );

        button.onclick =
            function () {

                window.location.href =
                    "/login.html";

            };

        return;
    }

    try {

        const response =
            await apiRequest(
                `/Favorites?page=1&pageSize=100`
            );

        if (!response.ok) {
            return;
        }

        const data =
            await response.json();

        const favorite =
            data.items?.find(
                x => x.bookId === bookId
            );

        if (favorite) {

            button.innerHTML =
                "♥ Ukloni iz favorita";

            button.classList.remove(
                "btn-outline-danger"
            );

            button.classList.add(
                "btn-danger"
            );

            button.onclick =
                function () {

                    removeFavorite(
                        favorite.id,
                        bookId
                    );

                };

        } else {

            button.innerHTML =
                "♡ Dodaj u favorite";

            button.classList.remove(
                "btn-danger"
            );

            button.classList.add(
                "btn-outline-danger"
            );

            button.onclick =
                function () {

                    toggleFavorite(
                        bookId
                    );

                };

        }

    } catch {
        
    }
}


async function toggleFavorite(bookId) {

    if (!isLoggedIn()) {

        window.location.href =
            "/login.html";

        return;
    }

    try {

        const response =
            await apiRequest(
                "/Favorites",
                {
                    method: "POST",

                    body: JSON.stringify({
                        userId: 0,
                        bookId: bookId
                    })
                }
            );

        if (response.status === 201) {

            await checkFavorite(
                bookId
            );

        } else if (
            response.status === 400
        ) {

            alert(
                "Knjiga je već u favoritima."
            );

        } else {

            alert(
                "Nije moguće dodati knjigu u favorite."
            );

        }

    } catch {

        alert(
            "Nije moguće dodati knjigu u favorite."
        );
    }
}


async function removeFavorite(
    favoriteId,
    bookId
) {

    try {

        const response =
            await apiRequest(
                `/Favorites/${favoriteId}`,
                {
                    method: "DELETE"
                }
            );

        if (response.ok) {

            await checkFavorite(
                bookId
            );

        } else {

            alert(
                "Nije moguće ukloniti favorite."
            );

        }

    } catch {

        alert(
            "Nije moguće ukloniti favorite."
        );
    }
}


// REVIEWS

async function loadReviews(bookId) {

    const container =
        document.getElementById(
            "reviewsContainer"
        );

    if (!container) {
        return;
    }

    try {

        const response =
            await apiRequest(
                `/Reviews?bookId=${bookId}&page=1&pageSize=100`
            );

        if (!response.ok) {

            container.innerHTML = `
                <div class="alert alert-danger">
                    Nije moguće učitati recenzije.
                </div>
            `;

            return;
        }

        const data =
            await response.json();

        if (
            !data.items ||
            data.items.length === 0
        ) {

            container.innerHTML = `
                <p class="text-muted">
                    Još nema recenzija.
                </p>
            `;

            return;
        }

        let html = "";

        for (const review of data.items) {

            html += `

                <div class="card mb-4 shadow-sm">

                    <div class="card-body">

                        <!-- REVIEW -->

                        <h5>
                            ${escapeHtml(
                review.title
            )}
                        </h5>

                        <div class="mb-2">

                            ${"★".repeat(
                review.rating
            )}

                            ${"☆".repeat(
                5 - review.rating
            )}

                        </div>

                        <p>
                            ${escapeHtml(
                review.text
            )}
                        </p>

                        <small class="text-muted">

                            ${new Date(
                review.createdAt
            ).toLocaleDateString()}

                        </small>


                        <!-- COMMENTS -->

                        <div class="mt-4">

                            <h6 class="mb-3">
                                💬 Komentari
                            </h6>

                            <div
                                id="comments-${review.id}"
                            >
                                <div class="text-muted">
                                    Učitavanje komentara...
                                </div>
                            </div>


                            <!-- ADD COMMENT -->

                            ${isLoggedIn()
                    ? `
                                        <form
                                            class="comment-form mt-3"
                                            data-review-id="${review.id}"
                                        >

                                            <div class="input-group">

                                                <input
                                                    type="text"
                                                    class="form-control comment-input"
                                                    placeholder="Napiši komentar..."
                                                    maxlength="1000"
                                                    required
                                                >

                                                <button
                                                    type="submit"
                                                    class="btn btn-outline-primary"
                                                >
                                                    Objavi
                                                </button>

                                            </div>

                                            <div
                                                class="comment-message mt-2"
                                            ></div>

                                        </form>
                                    `
                    : `
                                        <p class="text-muted mt-3 mb-0">
                                            Prijavi se da bi ostavila komentar.
                                        </p>
                                    `
                }

                        </div>

                    </div>

                </div>

            `;
        }

        container.innerHTML =
            html;


        // LOAD COMMENTS za svaki Review

        for (const review of data.items) {

            await loadComments(
                review.id
            );

        }


        // COMMENT FORMS

        document
            .querySelectorAll(".comment-form")
            .forEach(form => {

                form.addEventListener(
                    "submit",
                    submitComment
                );

            });

    } catch {

        container.innerHTML = `
            <div class="alert alert-danger">
                Greška prilikom učitavanja recenzija.
            </div>
        `;
    }
}


// LOAD COMMENTS

async function loadComments(reviewId) {

    const container =
        document.getElementById(
            `comments-${reviewId}`
        );

    if (!container) {
        return;
    }

    try {

        const response =
            await apiRequest(
                `/Comments?reviewId=${reviewId}&page=1&pageSize=100`
            );

        if (!response.ok) {

            container.innerHTML = `
                <div class="text-danger">
                    Nije moguće učitati komentare.
                </div>
            `;

            return;
        }

        const data =
            await response.json();

        if (
            !data.items ||
            data.items.length === 0
        ) {

            container.innerHTML = `
                <p class="text-muted small">
                    Još nema komentara.
                </p>
            `;

            return;
        }

        container.innerHTML =
            data.items.map(comment => `

                <div class="border rounded p-3 mb-2 bg-light">

                    <p class="mb-1">
                        ${escapeHtml(
                comment.text
            )}
                    </p>

                    <small class="text-muted">

                        ${new Date(
                comment.createdAt
            ).toLocaleDateString()}

                    </small>

                </div>

            `).join("");

    } catch {

        container.innerHTML = `
            <div class="text-danger">
                Greška prilikom učitavanja komentara.
            </div>
        `;
    }
}


// ADD COMMENT

async function submitComment(event) {

    event.preventDefault();

    if (!isLoggedIn()) {

        window.location.href =
            "/login.html";

        return;
    }

    const form =
        event.currentTarget;

    const reviewId =
        Number(
            form.dataset.reviewId
        );

    const input =
        form.querySelector(
            ".comment-input"
        );

    const message =
        form.querySelector(
            ".comment-message"
        );

    const text =
        input.value.trim();

    if (!reviewId || !text) {

        if (message) {

            message.innerHTML = `
                <div class="alert alert-danger py-2">
                    Unesi komentar.
                </div>
            `;

        }

        return;
    }

    try {

        const response =
            await apiRequest(
                "/Comments",
                {
                    method: "POST",

                    body: JSON.stringify({

                        reviewId: reviewId,

                        userId: 0,

                        text: text

                    })
                }
            );

        if (!response.ok) {

            const errorText =
                await response.text();

            console.error(
                "Greška Comments:",
                errorText
            );

            if (message) {

                message.innerHTML = `
                    <div class="alert alert-danger py-2">
                        Nije moguće dodati komentar.
                    </div>
                `;

            }

            return;
        }

        input.value = "";

        if (message) {

            message.innerHTML = `
                <div class="alert alert-success py-2">
                    Komentar je dodat.
                </div>
            `;

        }

        await loadComments(
            reviewId
        );

    } catch {

        if (message) {

            message.innerHTML = `
                <div class="alert alert-danger py-2">
                    Greška prilikom dodavanja komentara.
                </div>
            `;

        }
    }
}


// ADD REVIEW

async function submitReview(event) {

    event.preventDefault();

    if (!isLoggedIn()) {

        window.location.href =
            "/login.html";

        return;
    }

    const params =
        new URLSearchParams(
            window.location.search
        );

    const bookId =
        Number(
            params.get("id")
        );

    const title =
        document
            .getElementById("reviewTitle")
            ?.value
            .trim() || "";

    const text =
        document
            .getElementById("reviewText")
            ?.value
            .trim() || "";

    const ratingElement =
        document.getElementById(
            "reviewRating"
        );

    const ratingValue =
        ratingElement?.value;

    if (
        !bookId ||
        !title ||
        !text ||
        !ratingValue
    ) {

        showMessage(
            "reviewMessage",
            "Popuni sva polja.",
            "danger"
        );

        return;
    }

    const rating =
        parseInt(
            ratingValue,
            10
        );

    if (
        isNaN(rating) ||
        rating < 1 ||
        rating > 5
    ) {

        showMessage(
            "reviewMessage",
            "Ocena mora biti između 1 i 5.",
            "danger"
        );

        return;
    }

    try {

        const data = {

            bookId: bookId,

            userId: 0,

            title: title,

            text: text,

            rating: rating

        };

        console.log(
            "Šaljem recenziju:",
            data
        );

        const response =
            await apiRequest(
                "/Reviews",
                {
                    method: "POST",

                    body: JSON.stringify(data)
                }
            );

        if (!response.ok) {

            const errorText =
                await response.text();

            console.error(
                "Greška Reviews:",
                errorText
            );

            showMessage(
                "reviewMessage",
                "Nije moguće dodati recenziju.",
                "danger"
            );

            return;
        }

        showMessage(
            "reviewMessage",
            "Recenzija je uspešno dodata.",
            "success"
        );

        document
            .getElementById(
                "reviewForm"
            )
            ?.reset();

        await loadReviews(
            bookId
        );

    } catch (error) {

        console.error(error);

        showMessage(
            "reviewMessage",
            "Greška prilikom dodavanja recenzije.",
            "danger"
        );
    }
}


// SEARCH

function initializeBookSearch() {

    const searchInput =
        document.getElementById(
            "searchInput"
        );

    const searchButton =
        document.getElementById(
            "searchButton"
        );

    if (
        !searchInput ||
        !searchButton
    ) {
        return;
    }

    searchButton.addEventListener(
        "click",
        function () {

            currentSearch =
                searchInput.value.trim();

            loadBooks(1);

        }
    );

    searchInput.addEventListener(
        "keypress",
        function (e) {

            if (e.key === "Enter") {

                currentSearch =
                    searchInput.value.trim();

                loadBooks(1);

            }

        }
    );
}


// Page inicijalizacija

document.addEventListener(
    "DOMContentLoaded",
    () => {

        // BOOKS PAGE

        if (
            document.getElementById(
                "booksContainer"
            )
        ) {

            initializeBookSearch();

            loadCategories();

            loadBooks();

            const clearButton =
                document.getElementById(
                    "clearCategoryFilters"
                );

            if (clearButton) {

                clearButton.addEventListener(
                    "click",
                    clearCategoryFilters
                );

            }

        }


        // BOOK DETAILS PAGE

        if (
            document.getElementById(
                "bookDetails"
            )
        ) {

            loadBookDetails();

        }


        // REVIEW FORM

        const reviewForm =
            document.getElementById(
                "reviewForm"
            );

        if (reviewForm) {

            reviewForm.addEventListener(
                "submit",
                submitReview
            );

        }

    }
);