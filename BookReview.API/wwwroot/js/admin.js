let currentAdminSection = "books";

document.addEventListener("DOMContentLoaded", () => {

    const user = getCurrentUser();

    if (!user || user.role !== "Admin") {
        document.body.innerHTML = `
            <div class="container mt-5">

                <div class="alert alert-danger">
                    Nemaš dozvolu za pristup ovoj stranici.
                </div>

                <a href="/index.html" class="btn btn-primary">
                    Nazad
                </a>

            </div>
        `;

        return;
    }

    loadAdminBooks();
});


// BOOKS

async function loadAdminBooks() {

    currentAdminSection = "books";

    const container =
        document.getElementById("adminContent");

    if (!container) {
        return;
    }

    container.innerHTML = `
        <h3>Books</h3>

        <button
            class="btn btn-success mb-3"
            onclick="showBookForm()"
        >
            + Add book
        </button>

        <div id="adminTable"></div>
    `;

    try {

        const response = await apiRequest(
            "/Books?page=1&pageSize=100"
        );

        if (!response.ok) {

            container.innerHTML += `
                <div class="alert alert-danger">
                    Nije moguće učitati knjige.
                </div>
            `;

            return;
        }

        const data = await response.json();

        document.getElementById("adminTable").innerHTML = `
            <div class="table-responsive">

                <table class="table table-striped">

                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Title</th>
                            <th>ISBN</th>
                            <th>Year</th>
                            <th>Actions</th>
                        </tr>
                    </thead>

                    <tbody>

                        ${data.items.map(book => `
                            <tr>

                                <td>
                                    ${book.id}
                                </td>

                                <td>
                                    ${escapeHtml(book.title)}
                                </td>

                                <td>
                                    ${escapeHtml(book.isbn)}
                                </td>

                                <td>
                                    ${book.publishedYear}
                                </td>

                                <td>

                                    <button
                                        class="btn btn-sm btn-warning me-1"
                                        onclick="showEditBookForm(${book.id})"
                                    >
                                        Edit
                                    </button>

                                    <button
                                        class="btn btn-sm btn-danger"
                                        onclick="deleteBook(${book.id})"
                                    >
                                        Delete
                                    </button>

                                </td>

                            </tr>
                        `).join("")}

                    </tbody>

                </table>

            </div>
        `;

    } catch {

        document.getElementById("adminTable").innerHTML = `
            <div class="alert alert-danger">
                Greška.
            </div>
        `;
    }
}


// ADD BOOK

function showBookForm() {

    const container =
        document.getElementById("adminContent");

    container.insertAdjacentHTML("beforeend", `
        <div class="card shadow-sm mt-4">

            <div class="card-body">

                <h4>New book</h4>

                <form id="bookAdminForm">

                    <div class="mb-3">
                        <label class="form-label">
                            Title
                        </label>

                        <input
                            id="adminBookTitle"
                            class="form-control"
                            required
                        >
                    </div>

                    <div class="mb-3">
                        <label class="form-label">
                            Description
                        </label>

                        <textarea
                            id="adminBookDescription"
                            class="form-control"
                            rows="5"
                            required
                        ></textarea>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">
                            ISBN
                        </label>

                        <input
                            id="adminBookISBN"
                            class="form-control"
                            required
                        >
                    </div>

                    <div class="mb-3">
                        <label class="form-label">
                            Published year
                        </label>

                        <input
                            id="adminBookYear"
                            type="number"
                            class="form-control"
                            required
                        >
                    </div>

                    <div class="mb-3">
                        <label class="form-label">
                            Author
                        </label>

                        <select
                            id="adminBookAuthor"
                            class="form-select"
                            required
                        >
                            <option value="">
                                Select author
                            </option>
                        </select>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">
                            Category
                        </label>

                        <select
                            id="adminBookCategory"
                            class="form-select"
                            required
                        >
                            <option value="">
                                Select category
                            </option>
                        </select>
                    </div>

                    <div class="mb-3">

                        <label class="form-label">
                            Book cover
                        </label>

                        <input
                            id="adminBookCover"
                            type="file"
                            class="form-control"
                            accept="image/jpeg,image/png,image/webp"
                        >

                        <small class="text-muted">
                            JPG, JPEG, PNG ili WEBP. Maksimalno 5 MB.
                        </small>

                    </div>

                    <button
                        type="submit"
                        class="btn btn-success"
                    >
                        Save
                    </button>

                    <button
                        type="button"
                        class="btn btn-secondary ms-2"
                        onclick="loadAdminBooks()"
                    >
                        Cancel
                    </button>

                </form>

                <div
                    id="createBookMessage"
                    class="mt-3"
                ></div>

            </div>

        </div>
    `);

    loadBookFormData();

    document
        .getElementById("bookAdminForm")
        .addEventListener(
            "submit",
            createBook
        );
}


// LOAD AUTHORS + CATEGORIES

async function loadBookFormData() {

    try {

        const [
            authorsResponse,
            categoriesResponse
        ] = await Promise.all([
            apiRequest(
                "/Authors?page=1&pageSize=100"
            ),

            apiRequest(
                "/Categories?page=1&pageSize=100"
            )
        ]);

        if (
            !authorsResponse.ok ||
            !categoriesResponse.ok
        ) {
            alert(
                "Nije moguće učitati autore i kategorije."
            );

            return;
        }

        const authorsData =
            await authorsResponse.json();

        const categoriesData =
            await categoriesResponse.json();

        const authorSelect =
            document.getElementById(
                "adminBookAuthor"
            );

        const categorySelect =
            document.getElementById(
                "adminBookCategory"
            );

        if (!authorSelect || !categorySelect) {
            return;
        }

        authorSelect.innerHTML = `
            <option value="">
                Select author
            </option>

            ${authorsData.items.map(author => `
                <option value="${author.id}">
                    ${escapeHtml(author.firstName)}
                    ${escapeHtml(author.lastName)}
                </option>
            `).join("")}
        `;

        categorySelect.innerHTML = `
            <option value="">
                Select category
            </option>

            ${categoriesData.items.map(category => `
                <option value="${category.id}">
                    ${escapeHtml(category.name)}
                </option>
            `).join("")}
        `;

    } catch (error) {

        console.error(error);

        alert(
            "Greška prilikom učitavanja autora i kategorija."
        );
    }
}


// CREATE BOOK

async function createBook(event) {

    event.preventDefault();

    const message =
        document.getElementById(
            "createBookMessage"
        );

    const authorId =
        Number(
            document.getElementById(
                "adminBookAuthor"
            ).value
        );

    const categoryId =
        Number(
            document.getElementById(
                "adminBookCategory"
            ).value
        );

    const coverInput =
        document.getElementById(
            "adminBookCover"
        );

    const file =
        coverInput?.files[0];


    if (!authorId) {

        message.innerHTML = `
            <div class="alert alert-danger">
                Izaberi autora.
            </div>
        `;

        return;
    }


    if (!categoryId) {

        message.innerHTML = `
            <div class="alert alert-danger">
                Izaberi kategoriju.
            </div>
        `;

        return;
    }


    
    // CHECK IMAGE SIZE
    

    if (file && file.size > 5 * 1024 * 1024) {

        message.innerHTML = `
            <div class="alert alert-danger">
                Slika ne sme biti veća od 5 MB.
            </div>
        `;

        return;
    }


    const data = {

        title:
            document.getElementById(
                "adminBookTitle"
            ).value,

        description:
            document.getElementById(
                "adminBookDescription"
            ).value,

        isbn:
            document.getElementById(
                "adminBookISBN"
            ).value,

        publishedYear:
            Number(
                document.getElementById(
                    "adminBookYear"
                ).value
            ),

        authorId:
            authorId,

        categoryId:
            categoryId
    };


    try {

        
        // CREATE BOOK
        

        const response =
            await apiRequest(
                "/Books",
                {
                    method: "POST",
                    body: JSON.stringify(data)
                }
            );


        if (!response.ok) {

            const errorText =
                await response.text();

            console.error(
                "Greška pri kreiranju knjige:",
                errorText
            );

            message.innerHTML = `
                <div class="alert alert-danger">
                    Kreiranje knjige nije uspelo.
                </div>
            `;

            return;
        }


        
        // GET CREATED BOOK
        

        const createdBook =
            await response.json();

        const bookId =
            createdBook.id;


        
        // UPLOAD COVER
        

        if (file) {

            const formData =
                new FormData();

            formData.append(
                "file",
                file
            );


            const coverResponse =
                await apiRequest(
                    `/Books/${bookId}/cover`,
                    {
                        method: "POST",
                        body: formData
                    }
                );


            if (!coverResponse.ok) {

                const errorText =
                    await coverResponse.text();

                console.error(
                    "Greška pri uploadu slike:",
                    errorText
                );

                message.innerHTML = `
                    <div class="alert alert-warning">
                        Knjiga je kreirana,
                        ali slika nije sačuvana.
                    </div>
                `;

                await loadAdminBooks();

                return;
            }
        }


        // SUCCESS
        
        await loadAdminBooks();

    } catch (error) {

        console.error(error);

        message.innerHTML = `
            <div class="alert alert-danger">
                Greška prilikom kreiranja knjige.
            </div>
        `;
    }
}


// EDIT BOOK

async function showEditBookForm(id) {

    try {

        const response =
            await apiRequest(
                `/Books/${id}`
            );

        if (!response.ok) {

            alert(
                "Nije moguće učitati knjigu."
            );

            return;
        }

        const book =
            await response.json();

        const container =
            document.getElementById(
                "adminContent"
            );

        const currentCover =
            book.coverImagePath
                ? book.coverImagePath
                : "https://via.placeholder.com/150x200?text=No+Cover";


        container.insertAdjacentHTML(
            "beforeend",
            `

            <div class="card shadow-sm mt-4">

                <div class="card-body">

                    <h4>Edit book</h4>

                    <form id="editBookForm">

                        <div class="mb-3">

                            <label class="form-label">
                                Title
                            </label>

                            <input
                                id="editBookTitle"
                                class="form-control"
                                value="${escapeHtml(book.title)}"
                                required
                            >

                        </div>


                        <div class="mb-3">

                            <label class="form-label">
                                Description
                            </label>

                            <textarea
                                id="editBookDescription"
                                class="form-control"
                                rows="5"
                                required
                            >${escapeHtml(book.description)}</textarea>

                        </div>


                        <div class="mb-3">

                            <label class="form-label">
                                ISBN
                            </label>

                            <input
                                id="editBookISBN"
                                class="form-control"
                                value="${escapeHtml(book.isbn)}"
                                required
                            >

                        </div>


                        <div class="mb-3">

                            <label class="form-label">
                                Published year
                            </label>

                            <input
                                id="editBookYear"
                                type="number"
                                class="form-control"
                                value="${book.publishedYear}"
                                required
                            >

                        </div>


                        <div class="mb-3">

                            <label class="form-label">
                                Current cover
                            </label>

                            <div class="mb-3">

                                <img
                                    src="${currentCover}"
                                    alt="Book cover"
                                    style="
                                        width: 150px;
                                        height: 200px;
                                        object-fit: cover;
                                    "
                                    class="rounded shadow-sm"
                                >

                            </div>


                            <label
                                for="editBookCover"
                                class="form-label"
                            >
                                Change cover
                            </label>

                            <input
                                id="editBookCover"
                                type="file"
                                class="form-control"
                                accept="image/jpeg,image/png,image/webp"
                            >

                            <small class="text-muted">
                                JPG, JPEG, PNG ili WEBP. Maksimalno 5 MB.
                            </small>

                        </div>


                        <button
                            type="submit"
                            class="btn btn-warning"
                        >
                            Save changes
                        </button>


                        <button
                            type="button"
                            class="btn btn-secondary ms-2"
                            onclick="loadAdminBooks()"
                        >
                            Cancel
                        </button>

                    </form>


                    <div
                        id="editBookMessage"
                        class="mt-3"
                    ></div>

                </div>

            </div>

        `
        );


        document
            .getElementById("editBookForm")
            .addEventListener(
                "submit",
                async function (event) {

                    event.preventDefault();


                    const message =
                        document.getElementById(
                            "editBookMessage"
                        );


                    const data = {

                        title:
                            document.getElementById(
                                "editBookTitle"
                            ).value,

                        description:
                            document.getElementById(
                                "editBookDescription"
                            ).value,

                        isbn:
                            document.getElementById(
                                "editBookISBN"
                            ).value,

                        publishedYear:
                            Number(
                                document.getElementById(
                                    "editBookYear"
                                ).value
                            )
                    };


                    try {

                        
                        // UPDATE BOOK DATA
                       

                        const updateResponse =
                            await apiRequest(
                                `/Books/${id}`,
                                {
                                    method: "PUT",
                                    body: JSON.stringify(data)
                                }
                            );


                        if (!updateResponse.ok) {

                            const errorText =
                                await updateResponse.text();

                            console.error(
                                "Greška pri izmeni knjige:",
                                errorText
                            );

                            message.innerHTML = `
                                <div class="alert alert-danger">
                                    Izmena knjige nije uspela.
                                </div>
                            `;

                            return;
                        }


                        
                        // UPDATE COVER
                       

                        const coverInput =
                            document.getElementById(
                                "editBookCover"
                            );

                        const file =
                            coverInput.files[0];


                        if (file) {

                            if (
                                file.size >
                                5 * 1024 * 1024
                            ) {

                                message.innerHTML = `
                                    <div class="alert alert-danger">
                                        Slika ne sme biti veća od 5 MB.
                                    </div>
                                `;

                                return;
                            }


                            const formData =
                                new FormData();

                            formData.append(
                                "file",
                                file
                            );


                            const coverResponse =
                                await apiRequest(
                                    `/Books/${id}/cover`,
                                    {
                                        method: "POST",
                                        body: formData
                                    }
                                );


                            if (!coverResponse.ok) {

                                const errorText =
                                    await coverResponse.text();

                                console.error(
                                    "Greška pri uploadu slike:",
                                    errorText
                                );

                                message.innerHTML = `
                                    <div class="alert alert-warning">
                                        Podaci knjige su izmenjeni,
                                        ali nova naslovna slika nije sačuvana.
                                    </div>
                                `;

                                return;
                            }
                        }


                        
                        // SUCCESS
                        

                        await loadAdminBooks();

                    } catch (error) {

                        console.error(error);

                        message.innerHTML = `
                            <div class="alert alert-danger">
                                Greška prilikom izmene knjige.
                            </div>
                        `;
                    }
                }
            );

    } catch (error) {

        console.error(error);

        alert(
            "Greška prilikom učitavanja knjige."
        );
    }
}


// DELETE BOOK


async function deleteBook(id) {

    if (!confirm(
        "Da li sigurno želiš da obrišeš knjigu?"
    )) {
        return;
    }


    try {

        const response =
            await apiRequest(
                `/Books/${id}`,
                {
                    method: "DELETE"
                }
            );


        if (!response.ok) {

            alert(
                "Brisanje nije uspelo."
            );

            return;
        }


        await loadAdminBooks();

    } catch {

        alert("Greška.");
    }
}


// AUTHORS


async function loadAdminAuthors() {

    currentAdminSection = "authors";

    const container =
        document.getElementById(
            "adminContent"
        );


    container.innerHTML = `

        <h3>Authors</h3>

        <button
            class="btn btn-success mb-3"
            onclick="showAuthorForm()"
        >
            + Add author
        </button>

        <div id="adminTable"></div>

    `;


    try {

        const response =
            await apiRequest(
                "/Authors?page=1&pageSize=100"
            );


        if (!response.ok) {

            document.getElementById(
                "adminTable"
            ).innerHTML = `

                <div class="alert alert-danger">
                    Nije moguće učitati autore.
                </div>

            `;

            return;
        }


        const data =
            await response.json();


        document.getElementById(
            "adminTable"
        ).innerHTML = `

            <div class="table-responsive">

                <table class="table table-striped">

                    <thead>

                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>Actions</th>
                        </tr>

                    </thead>


                    <tbody>

                        ${data.items.map(author => `

                            <tr>

                                <td>
                                    ${author.id}
                                </td>

                                <td>
                                    ${escapeHtml(author.firstName)}
                                    ${escapeHtml(author.lastName)}
                                </td>

                                <td>

                                    <button
                                        class="btn btn-sm btn-warning me-1"
                                        onclick="showEditAuthorForm(${author.id})"
                                    >
                                        Edit
                                    </button>


                                    <button
                                        class="btn btn-sm btn-danger"
                                        onclick="deleteAuthor(${author.id})"
                                    >
                                        Delete
                                    </button>

                                </td>

                            </tr>

                        `).join("")}

                    </tbody>

                </table>

            </div>

        `;

    } catch {

        document.getElementById(
            "adminTable"
        ).innerHTML = `

            <div class="alert alert-danger">
                Greška.
            </div>

        `;
    }
}


// ADD AUTHOR

function showAuthorForm() {

    const container =
        document.getElementById(
            "adminContent"
        );


    container.insertAdjacentHTML(
        "beforeend",
        `

        <div class="card mt-3">

            <div class="card-body">

                <h4>New author</h4>

                <form id="authorForm">

                    <input
                        id="authorFirstName"
                        class="form-control mb-2"
                        placeholder="First name"
                        required
                    >

                    <input
                        id="authorLastName"
                        class="form-control mb-2"
                        placeholder="Last name"
                        required
                    >

                    <textarea
                        id="authorBiography"
                        class="form-control mb-2"
                        placeholder="Biography"
                    ></textarea>

                    <button class="btn btn-success">
                        Save
                    </button>

                </form>

            </div>

        </div>

    `
    );


    document
        .getElementById("authorForm")
        .addEventListener(
            "submit",
            createAuthor
        );
}


// CREATE AUTHOR

async function createAuthor(event) {

    event.preventDefault();


    const data = {

        firstName:
            document.getElementById(
                "authorFirstName"
            ).value,

        lastName:
            document.getElementById(
                "authorLastName"
            ).value,

        biography:
            document.getElementById(
                "authorBiography"
            ).value
    };


    try {

        const response =
            await apiRequest(
                "/Authors",
                {
                    method: "POST",
                    body: JSON.stringify(data)
                }
            );


        if (response.ok) {

            await loadAdminAuthors();

        } else {

            alert(
                "Kreiranje autora nije uspelo."
            );
        }

    } catch {

        alert("Greška.");
    }
}


// EDIT AUTHOR

async function showEditAuthorForm(id) {

    try {

        const response =
            await apiRequest(
                `/Authors/${id}`
            );


        if (!response.ok) {

            alert(
                "Nije moguće učitati autora."
            );

            return;
        }


        const author =
            await response.json();


        const container =
            document.getElementById(
                "adminContent"
            );


        container.insertAdjacentHTML(
            "beforeend",
            `

            <div class="card shadow-sm mt-3">

                <div class="card-body">

                    <h4>Edit author</h4>

                    <form id="editAuthorForm">

                        <input
                            id="editAuthorFirstName"
                            class="form-control mb-2"
                            value="${escapeHtml(author.firstName)}"
                            required
                        >

                        <input
                            id="editAuthorLastName"
                            class="form-control mb-2"
                            value="${escapeHtml(author.lastName)}"
                            required
                        >

                        <textarea
                            id="editAuthorBiography"
                            class="form-control mb-2"
                            required
                        >${escapeHtml(author.biography || "")}</textarea>


                        <button
                            class="btn btn-warning"
                            type="submit"
                        >
                            Save changes
                        </button>


                        <button
                            class="btn btn-secondary ms-2"
                            type="button"
                            onclick="loadAdminAuthors()"
                        >
                            Cancel
                        </button>

                    </form>

                </div>

            </div>

        `
        );


        document
            .getElementById("editAuthorForm")
            .addEventListener(
                "submit",
                async function (event) {

                    event.preventDefault();


                    const data = {

                        firstName:
                            document.getElementById(
                                "editAuthorFirstName"
                            ).value,

                        lastName:
                            document.getElementById(
                                "editAuthorLastName"
                            ).value,

                        biography:
                            document.getElementById(
                                "editAuthorBiography"
                            ).value
                    };


                    try {

                        const updateResponse =
                            await apiRequest(
                                `/Authors/${id}`,
                                {
                                    method: "PUT",
                                    body: JSON.stringify(data)
                                }
                            );


                        if (!updateResponse.ok) {

                            alert(
                                "Izmena autora nije uspela."
                            );

                            return;
                        }


                        await loadAdminAuthors();

                    } catch {

                        alert("Greška.");
                    }
                }
            );

    } catch {

        alert(
            "Greška prilikom učitavanja autora."
        );
    }
}


// DELETE AUTHOR

async function deleteAuthor(id) {

    if (!confirm(
        "Obrisati autora?"
    )) {
        return;
    }


    try {

        const response =
            await apiRequest(
                `/Authors/${id}`,
                {
                    method: "DELETE"
                }
            );


        if (response.ok) {

            await loadAdminAuthors();

        } else {

            alert(
                "Brisanje autora nije uspelo."
            );
        }

    } catch {

        alert("Greška.");
    }
}


// CATEGORIES

async function loadAdminCategories() {

    currentAdminSection = "categories";

    const container =
        document.getElementById(
            "adminContent"
        );


    container.innerHTML = `

        <h3>Categories</h3>

        <button
            class="btn btn-success mb-3"
            onclick="showCategoryForm()"
        >
            + Add category
        </button>

        <div id="adminTable"></div>

    `;


    try {

        const response =
            await apiRequest(
                "/Categories?page=1&pageSize=100"
            );


        if (!response.ok) {

            document.getElementById(
                "adminTable"
            ).innerHTML = `

                <div class="alert alert-danger">
                    Nije moguće učitati kategorije.
                </div>

            `;

            return;
        }


        const data =
            await response.json();


        document.getElementById(
            "adminTable"
        ).innerHTML = `

            <div class="table-responsive">

                <table class="table table-striped">

                    <thead>

                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>Actions</th>
                        </tr>

                    </thead>


                    <tbody>

                        ${data.items.map(category => `

                            <tr>

                                <td>
                                    ${category.id}
                                </td>

                                <td>
                                    ${escapeHtml(category.name)}
                                </td>

                                <td>

                                    <button
                                        class="btn btn-sm btn-warning me-1"
                                        onclick="showEditCategoryForm(${category.id})"
                                    >
                                        Edit
                                    </button>


                                    <button
                                        class="btn btn-sm btn-danger"
                                        onclick="deleteCategory(${category.id})"
                                    >
                                        Delete
                                    </button>

                                </td>

                            </tr>

                        `).join("")}

                    </tbody>

                </table>

            </div>

        `;

    } catch {

        document.getElementById(
            "adminTable"
        ).innerHTML = `

            <div class="alert alert-danger">
                Greška.
            </div>

        `;
    }
}


// ADD CATEGORY

function showCategoryForm() {

    const container =
        document.getElementById(
            "adminContent"
        );


    container.insertAdjacentHTML(
        "beforeend",
        `

        <div class="card mt-3">

            <div class="card-body">

                <h4>New category</h4>

                <form id="categoryForm">

                    <input
                        id="categoryName"
                        class="form-control mb-2"
                        placeholder="Category name"
                        required
                    >

                    <button class="btn btn-success">
                        Save
                    </button>

                </form>

            </div>

        </div>

    `
    );


    document
        .getElementById("categoryForm")
        .addEventListener(
            "submit",
            createCategory
        );
}


// CREATE CATEGORY


async function createCategory(event) {

    event.preventDefault();


    const data = {

        name:
            document.getElementById(
                "categoryName"
            ).value
    };


    try {

        const response =
            await apiRequest(
                "/Categories",
                {
                    method: "POST",
                    body: JSON.stringify(data)
                }
            );


        if (response.ok) {

            await loadAdminCategories();

        } else {

            alert(
                "Kreiranje kategorije nije uspelo."
            );
        }

    } catch {

        alert("Greška.");
    }
}


// EDIT CATEGORY

async function showEditCategoryForm(id) {

    try {

        const response =
            await apiRequest(
                `/Categories/${id}`
            );


        if (!response.ok) {

            alert(
                "Nije moguće učitati kategoriju."
            );

            return;
        }


        const category =
            await response.json();


        const container =
            document.getElementById(
                "adminContent"
            );


        container.insertAdjacentHTML(
            "beforeend",
            `

            <div class="card shadow-sm mt-3">

                <div class="card-body">

                    <h4>Edit category</h4>

                    <form id="editCategoryForm">

                        <input
                            id="editCategoryName"
                            class="form-control mb-2"
                            value="${escapeHtml(category.name)}"
                            required
                        >


                        <button
                            class="btn btn-warning"
                            type="submit"
                        >
                            Save changes
                        </button>


                        <button
                            class="btn btn-secondary ms-2"
                            type="button"
                            onclick="loadAdminCategories()"
                        >
                            Cancel
                        </button>

                    </form>

                </div>

            </div>

        `
        );


        document
            .getElementById("editCategoryForm")
            .addEventListener(
                "submit",
                async function (event) {

                    event.preventDefault();


                    const data = {

                        name:
                            document.getElementById(
                                "editCategoryName"
                            ).value
                    };


                    try {

                        const updateResponse =
                            await apiRequest(
                                `/Categories/${id}`,
                                {
                                    method: "PUT",
                                    body: JSON.stringify(data)
                                }
                            );


                        if (!updateResponse.ok) {

                            alert(
                                "Izmena kategorije nije uspela."
                            );

                            return;
                        }


                        await loadAdminCategories();

                    } catch {

                        alert("Greška.");
                    }
                }
            );

    } catch {

        alert(
            "Greška prilikom učitavanja kategorije."
        );
    }
}


// DELETE CATEGORY

async function deleteCategory(id) {

    if (!confirm(
        "Obrisati kategoriju?"
    )) {
        return;
    }


    try {

        const response =
            await apiRequest(
                `/Categories/${id}`,
                {
                    method: "DELETE"
                }
            );


        if (response.ok) {

            await loadAdminCategories();

        } else {

            alert(
                "Brisanje nije uspelo."
            );
        }

    } catch {

        alert("Greška.");
    }
}