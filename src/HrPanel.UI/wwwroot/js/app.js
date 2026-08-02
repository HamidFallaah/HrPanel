(() => {
    "use strict";

    const appShell = document.querySelector("[data-app-shell]");
    const sidebarToggle = document.querySelector("[data-sidebar-toggle]");
    const sidebarStateKey = "hrpanel.ui.sidebar-collapsed";
    const persianDigits = "۰۱۲۳۴۵۶۷۸۹";

    const safelyReadSidebarState = () => {
        try {
            return window.localStorage.getItem(sidebarStateKey) === "true";
        } catch {
            return false;
        }
    };

    const safelyStoreSidebarState = (isCollapsed) => {
        try {
            window.localStorage.setItem(sidebarStateKey, String(isCollapsed));
        } catch {
            // The layout remains functional when browser storage is unavailable.
        }
    };

    const updateSidebar = (isCollapsed) => {
        if (!appShell || !sidebarToggle) {
            return;
        }

        appShell.classList.toggle("is-sidebar-collapsed", isCollapsed);
        sidebarToggle.setAttribute("aria-expanded", String(!isCollapsed));
        sidebarToggle.setAttribute("title", isCollapsed ? "بازکردن منو" : "جمع‌کردن منو");
    };

    updateSidebar(safelyReadSidebarState());

    sidebarToggle?.addEventListener("click", () => {
        const isCollapsed = !appShell?.classList.contains("is-sidebar-collapsed");
        updateSidebar(isCollapsed);
        safelyStoreSidebarState(isCollapsed);
    });

    document.querySelectorAll("#mobile-navigation .navigation-link").forEach((link) => {
        link.addEventListener("click", () => {
            const navigation = document.getElementById("mobile-navigation");
            const instance = navigation && window.bootstrap
                ? window.bootstrap.Offcanvas.getInstance(navigation)
                : null;

            instance?.hide();
        });
    });

    document.querySelectorAll("[data-persian-number]").forEach((element) => {
        element.textContent = (element.textContent ?? "").replace(/\d/g, (digit) => persianDigits[Number(digit)]);
    });

    document.querySelectorAll("[data-retry-page]").forEach((button) => {
        button.addEventListener("click", () => window.location.reload());
    });

    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach((element) => {
        if (window.bootstrap) {
            window.bootstrap.Tooltip.getOrCreateInstance(element);
        }
    });

    document.querySelectorAll(".org-tree > li").forEach((item) => {
        const trigger = item.querySelector(":scope > span");
        const children = item.querySelector(":scope > ul");
        if (!trigger || !children) {
            return;
        }

        trigger.setAttribute("role", "button");
        trigger.setAttribute("tabindex", "0");
        trigger.setAttribute("aria-expanded", "true");
        const toggle = () => {
            const collapsed = item.classList.toggle("is-collapsed");
            trigger.setAttribute("aria-expanded", String(!collapsed));
        };
        trigger.addEventListener("click", toggle);
        trigger.addEventListener("keydown", (event) => {
            if (event.key === "Enter" || event.key === " ") {
                event.preventDefault();
                toggle();
            }
        });
    });

    document.querySelectorAll("form[data-prevent-double-submit]").forEach((form) => {
        form.addEventListener("submit", () => {
            if (!form.checkValidity()) {
                return;
            }

            window.setTimeout(() => {
                form.querySelectorAll('button[type="submit"], input[type="submit"]').forEach((submitButton) => {
                    submitButton.disabled = true;
                    submitButton.setAttribute("aria-disabled", "true");
                });
            }, 0);
        });
    });

    document.querySelectorAll("form[data-confirm]").forEach((form) => {
        form.addEventListener("submit", async (event) => {
            if (form.dataset.confirmed === "true") {
                return;
            }

            event.preventDefault();
            const confirmed = await confirmAction({
                title: form.dataset.confirm || "تأیید عملیات",
                text: "پس از تأیید، اطلاعات سامانه به‌روزرسانی می‌شود."
            });

            if (confirmed) {
                form.dataset.confirmed = "true";
                form.requestSubmit();
            }
        });
    });

    if (window.jalaliDatepicker) {
        window.jalaliDatepicker.startWatch({
            autoHide: true,
            hideAfterChange: true,
            persianDigits: true,
            useDropDownYears: true
        });
    }

    const getCsrfToken = () => document.querySelector('meta[name="csrf-token"]')?.getAttribute("content") ?? "";

    const fetchWithAntiforgery = (input, init = {}) => {
        const method = (init.method ?? "GET").toUpperCase();
        const headers = new Headers(init.headers ?? {});

        if (!["GET", "HEAD", "OPTIONS", "TRACE"].includes(method)) {
            const token = getCsrfToken();
            if (token) {
                headers.set("X-CSRF-TOKEN", token);
            }
        }

        return window.fetch(input, {
            credentials: "same-origin",
            ...init,
            headers
        });
    };

    const confirmAction = async ({
        title = "از انجام این عملیات مطمئن هستید؟",
        text = "این عملیات پس از تأیید انجام می‌شود.",
        confirmButtonText = "تأیید",
        cancelButtonText = "انصراف",
        icon = "warning"
    } = {}) => {
        if (!window.Swal) {
            return window.confirm(`${title}\n${text}`);
        }

        const result = await window.Swal.fire({
            title,
            text,
            icon,
            showCancelButton: true,
            reverseButtons: true,
            focusCancel: true,
            confirmButtonText,
            cancelButtonText
        });

        return result.isConfirmed;
    };

    window.HrPanel = Object.freeze({
        confirmAction,
        fetch: fetchWithAntiforgery,
        getCsrfToken,
        toPersianDigits(value) {
            return String(value ?? "").replace(/\d/g, (digit) => persianDigits[Number(digit)]);
        }
    });
})();
