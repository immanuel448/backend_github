document.addEventListener("DOMContentLoaded", async () => {
  const overlay = document.createElement("div");
  overlay.id = "overlay-acceso";
  overlay.innerHTML = `
    <div class="acceso-contenedor">
      <h2>Acceso privado</h2>
      <p>Introduce la contraseña para continuar</p>
      <input type="password" id="claveAcceso" placeholder="Contraseña" />
      <button id="btnAcceder">Entrar</button>
      <div id="msgAcceso"></div>
    </div>
  `;
  document.body.appendChild(overlay);

  const style = document.createElement("style");
  style.textContent = `
    #overlay-acceso {
      position: fixed;
      inset: 0;
      background: rgba(255,255,255,0.97);
      backdrop-filter: blur(4px);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 9999;
      transition: opacity 0.5s ease;
    }
    .acceso-contenedor {
      background: white;
      padding: 2rem 3rem;
      border-radius: 12px;
      box-shadow: 0 0 15px rgba(0,0,0,0.2);
      text-align: center;
      font-family: system-ui, sans-serif;
    }
  `;
  document.head.appendChild(style);

  try {
    const res = await fetch("/api/auth/verificarSesion");
    const data = await res.json();
    if (data.autenticado) {
      overlay.style.display = "none";
      return;
    }
  } catch (err) {
    console.error("Error al verificar sesión:", err);
  }

  document.getElementById("btnAcceder").addEventListener("click", async () => {
    const clave = document.getElementById("claveAcceso").value.trim();
    const msg = document.getElementById("msgAcceso");

    if (!clave) {
      msg.textContent = "Introduce la contraseña.";
      msg.style.color = "red";
      return;
    }

    try {
      const res = await fetch("/api/auth/verificar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ clave })
      });
      const data = await res.json();

      if (data.acceso) {
        msg.textContent = "✅ Acceso permitido";
        msg.style.color = "green";
        overlay.style.opacity = 0;
        setTimeout(() => (overlay.style.display = "none"), 500);
      } else {
        msg.textContent = "❌ Contraseña incorrecta";
        msg.style.color = "red";
      }
    } catch (err) {
      msg.textContent = "⚠️ Error de conexión.";
      msg.style.color = "red";
    }
  });

  const btnLogout = document.querySelector("#cerrar-sesion");
  if (btnLogout) {
    btnLogout.addEventListener("click", async () => {
      await fetch("/api/auth/salir", { method: "POST" });
      alert("Sesión cerrada.");
      location.reload();
    });
  }
});
