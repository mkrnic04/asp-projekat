//console.log("API");
const API_BASE = "/api";

function getToken() {
    return localStorage.getItem("token");
}

function getRefreshToken() {
    return localStorage.getItem("refreshToken");
}

function saveTokens(data) {
    if (data.token) {
        localStorage.setItem("token", data.token);
    }

    if (data.refreshToken) {
        localStorage.setItem("refreshToken", data.refreshToken);
    }
}

function clearTokens() {
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
}

function isLoggedIn() {
    return !!getToken();
}

function getCurrentUser() {
    const token = getToken();

    if (!token) {
        return null;
    }

    try {
        const payload = JSON.parse(atob(token.split(".")[1]));

        return {
            id: payload.Id,
            username: payload.Username,
            firstName: payload.FirstName,
            lastName: payload.LastName,
            email: payload.Email,
            role: payload.Role
        };
    } catch {
        return null;
    }
}

async function apiRequest(url, options = {}) {
    const token = getToken();

    const headers = {
        ...options.headers
    };

    if (!(options.body instanceof FormData)) {
        headers["Content-Type"] = "application/json";
    }

    if (token) {
        headers["Authorization"] = `Bearer ${token}`;
    }

    let response = await fetch(API_BASE + url, {
        ...options,
        headers
    });

    // Ako je token istekao, refresh
    if (response.status === 401 && getRefreshToken()) {
        const refreshed = await refreshToken();

        if (refreshed) {
            headers["Authorization"] = `Bearer ${getToken()}`;

            response = await fetch(API_BASE + url, {
                ...options,
                headers
            });
        }
    }

    return response;
}

async function refreshToken() {
    const refresh = getRefreshToken();

    if (!refresh) {
        return false;
    }

    try {
        const response = await fetch(`${API_BASE}/Auth/refresh`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                refreshToken: refresh
            })
        });

        if (!response.ok) {
            clearTokens();
            return false;
        }

        const data = await response.json();

        saveTokens(data);

        return true;
    } catch {
        clearTokens();
        return false;
    }
}

async function logout() {
    try {
        if (getToken()) {
            await apiRequest("/Auth/logout", {
                method: "POST"
            });
        }
    } catch {
       
    }

    clearTokens();

    window.location.href = "/index.html";
}

function showMessage(elementId, message, type = "danger") {
    const element = document.getElementById(elementId);

    if (!element) {
        return;
    }

    element.innerHTML = `
        <div class="alert alert-${type}">
            ${message}
        </div>
    `;
}

function escapeHtml(text) {
    if (text === null || text === undefined) {
        return "";
    }

    return String(text)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

function updateNavbar() {

    const navbarLinks =
        document.getElementById("navbarLinks");

    if (!navbarLinks) {
        return;
    }

    const user = getCurrentUser();

    let html = `
        <a class="nav-link" href="/index.html">
            Home
        </a>

        <a class="nav-link" href="/books.html">
            Books
        </a>
    `;

    if (user) {

        html += `
            <a class="nav-link" href="/favorites.html">
                Favorites
            </a>

            <a class="nav-link" href="/profile.html">
                Profile
            </a>
        `;

        if (user.role === "Admin") {

            html += `
                <a class="nav-link" href="/admin.html">
                    Admin Panel
                </a>
            `;
        }

        html += `
            <a class="nav-link" href="#" id="logoutButton">
                Logout
            </a>
        `;

    } else {

        html += `
            <a class="nav-link" href="/login.html">
                Login
            </a>

            <a class="nav-link" href="/register.html">
                Register
            </a>
        `;
    }

    navbarLinks.innerHTML = html;


    const logoutButton =
        document.getElementById("logoutButton");

    if (logoutButton) {

        logoutButton.addEventListener(
            "click",
            function (e) {

                e.preventDefault();

                logout();

            }
        );
    }
} function updateNavbar() {

    const navbarLinks =
        document.getElementById("navbarLinks");

    if (!navbarLinks) {
        return;
    }

    const user = getCurrentUser();

    let html = `
        <a class="nav-link" href="/index.html">
            Home
        </a>

        <a class="nav-link" href="/books.html">
            Books
        </a>
    `;

    if (user) {

        html += `
            <a class="nav-link" href="/favorites.html">
                Favorites
            </a>

            <a class="nav-link" href="/profile.html">
                Profile
            </a>
        `;

        if (user.role === "Admin") {

            html += `
                <a class="nav-link" href="/admin.html">
                    Admin Panel
                </a>
            `;
        }

        html += `
            <a class="nav-link" href="#" id="logoutButton">
                Logout
            </a>
        `;

    } else {

        html += `
            <a class="nav-link" href="/login.html">
                Login
            </a>

            <a class="nav-link" href="/register.html">
                Register
            </a>
        `;
    }

    navbarLinks.innerHTML = html;


    const logoutButton =
        document.getElementById("logoutButton");

    if (logoutButton) {

        logoutButton.addEventListener(
            "click",
            function (e) {

                e.preventDefault();

                logout();

            }
        );
    }
}

document.addEventListener("DOMContentLoaded", updateNavbar);