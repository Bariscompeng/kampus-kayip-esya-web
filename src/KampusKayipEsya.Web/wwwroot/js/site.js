// Kampüs Kayıp Eşya · ortak etkileşimler

// Gönderilen POST formlarının butonu meşgul durumuna geçer: çift gönderim engellenir,
// kullanıcı isteğin başladığını görür. İstemci doğrulaması gönderimi durdurursa buton açık kalır.
document.addEventListener("submit", olay => {
    const form = olay.target;
    if (!(form instanceof HTMLFormElement) || form.method.toLowerCase() !== "post") return;
    setTimeout(() => {
        if (olay.defaultPrevented) return;
        const buton = olay.submitter ?? form.querySelector("button[type=submit]");
        if (!buton || buton.disabled) return;
        buton.disabled = true;
        buton.classList.add("is-busy");
        buton.setAttribute("aria-busy", "true");
        const metin = buton.textContent.trim();
        if (metin) {
            buton.dataset.eskiMetin = buton.innerHTML;
            buton.textContent = metin.replace(/[.…]+$/, "") + "…";
        }
    });
});

// Sayfa geri tuşuyla önbellekten dönerse meşgul butonları eski haline getir.
window.addEventListener("pageshow", () => {
    document.querySelectorAll("button.is-busy").forEach(buton => {
        buton.disabled = false;
        buton.classList.remove("is-busy");
        buton.removeAttribute("aria-busy");
        if (buton.dataset.eskiMetin) buton.innerHTML = buton.dataset.eskiMetin;
    });
});

// ---------- Tema düğmesi: açık / koyu ----------
(() => {
    const dugme = document.getElementById("temaDugmesi");
    if (!dugme) return;
    const kok = document.documentElement;
    const hareketAzalt = matchMedia("(prefers-reduced-motion: reduce)");

    const etiketiGuncelle = () => {
        const acik = kok.dataset.theme === "light";
        dugme.setAttribute("aria-label", acik ? "Koyu temaya geç" : "Açık temaya geç");
        dugme.title = acik ? "Koyu tema" : "Açık tema";
    };

    const uygula = tema => {
        kok.dataset.theme = tema;
        kok.setAttribute("data-bs-theme", tema);
        const renk = document.querySelector('meta[name="theme-color"]');
        if (renk) renk.content = tema === "light" ? "#ffffff" : "#18181a";
        try { localStorage.setItem("tema", tema); } catch { }
        etiketiGuncelle();
    };

    etiketiGuncelle();
    dugme.addEventListener("click", () => {
        const yeni = kok.dataset.theme === "light" ? "dark" : "light";
        dugme.classList.remove("is-spinning");
        void dugme.offsetWidth; // ikon animasyonunu yeniden başlat
        dugme.classList.add("is-spinning");

        if (!document.startViewTransition || hareketAzalt.matches) { uygula(yeni); return; }

        // Yeni tema, düğmenin merkezinden ekranın en uzak köşesine kadar daire olarak yayılır.
        const r = dugme.getBoundingClientRect();
        const x = r.left + r.width / 2, y = r.top + r.height / 2;
        const yaricap = Math.hypot(Math.max(x, innerWidth - x), Math.max(y, innerHeight - y));
        kok.style.setProperty("--tx", `${x}px`);
        kok.style.setProperty("--ty", `${y}px`);
        kok.style.setProperty("--tr", `${yaricap}px`);
        kok.classList.add("tema-gecisi");
        const gecis = document.startViewTransition(() => uygula(yeni));
        gecis.ready.catch(() => { });
        gecis.finished.catch(() => { }).finally(() => kok.classList.remove("tema-gecisi"));
    });
})();

// ---------- Butonlarda tıklama dalgası ----------
// Dalga tıklanan noktadan yayılır; klavyeyle basılınca buton ortasından. Hareket azaltmada çalışmaz.
(() => {
    if (matchMedia("(prefers-reduced-motion: reduce)").matches) return;
    const dalga = (hedef, x, y) => {
        const r = hedef.getBoundingClientRect();
        const boy = Math.hypot(Math.max(x, r.width - x), Math.max(y, r.height - y)) * 2;
        const halka = document.createElement("span");
        halka.className = "ripple";
        halka.setAttribute("aria-hidden", "true");
        halka.style.width = halka.style.height = `${boy}px`;
        halka.style.left = `${x - boy / 2}px`;
        halka.style.top = `${y - boy / 2}px`;
        hedef.appendChild(halka);
        halka.addEventListener("animationend", () => halka.remove());
    };
    document.addEventListener("pointerdown", olay => {
        const hedef = olay.target.closest(".btn, .theme-toggle");
        if (!hedef || hedef.disabled || olay.button > 0) return;
        const r = hedef.getBoundingClientRect();
        dalga(hedef, olay.clientX - r.left, olay.clientY - r.top);
    });
    document.addEventListener("keydown", olay => {
        if (olay.key !== "Enter" && olay.key !== " ") return;
        const hedef = olay.target.closest?.(".btn, .theme-toggle");
        if (!hedef || hedef.disabled) return;
        const r = hedef.getBoundingClientRect();
        dalga(hedef, r.width / 2, r.height / 2);
    });
})();
