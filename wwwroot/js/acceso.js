// =======================================================
// acceso.js
// Controla el acceso y la sesión en páginas protegidas.
// =======================================================

document.addEventListener("DOMContentLoaded", async () => {
  // -------------------------------------------
  // 1️⃣ Verificar sesión activa
  // -------------------------------------------
  try {
    const res = await fetch("/api/auth/verificarSesion");
    const data = await res.json();

    if (!data.autenticado) {
      // No hay sesión activa → redirigir a la pantalla de acceso
      window.location.replace("/acceso.html");
      return;
    }

    // Si hay sesión activa, bloquear el caché y botón atrás
    window.history.pushState(null, "", window.location.href);
    window.onpopstate = function () {
      window.history.pushState(null, "", window.location.href);
    };

  } catch (err) {
    console.error("Error al verificar sesión:", err);
    window.location.replace("/acceso.html");
    return;
  }

  // -------------------------------------------
  // 2️⃣ Cerrar sesión manualmente
  // -------------------------------------------
  const btnLogout = document.getElementById("cerrar-sesion");
  if (btnLogout) {
    btnLogout.addEventListener("click", async () => {
      try {
        await fetch("/api/auth/salir", { method: "POST" });

        // Limpiar caché local
        sessionStorage.clear();
        localStorage.clear();

        // Redirigir al acceso
        window.location.replace("/acceso.html");
      } catch (err) {
        console.error("Error al cerrar sesión:", err);
      }
    });
  }
});
