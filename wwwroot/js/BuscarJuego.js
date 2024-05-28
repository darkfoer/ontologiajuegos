document.addEventListener('DOMContentLoaded', () => {
    const inputBuscar = document.getElementById('buscarJuego');
    const contenedorJuegos = document.getElementById('contenedorJuegos');

    inputBuscar.addEventListener('input', () => {
        const valorBuscar = inputBuscar.value.toLowerCase().trim();
        const url = `/Juegos/Search?query=${encodeURIComponent(valorBuscar)}`;

        fetch(url)
            .then(response => response.text())
            .then(html => {
                contenedorJuegos.innerHTML = html;

                // Actualizar la paginación
                const totalPaginas = parseInt(document.getElementById('totalPaginas').value, 10);
                const paginaActual = parseInt(document.getElementById('paginaActual').value, 10);
                const paginacion = document.querySelector('.pagination');
                paginacion.innerHTML = '';

                for (let i = 1; i <= totalPaginas; i++) {
                    const li = document.createElement('li');
                    li.classList.add('page-item', paginaActual === i ? 'active' : '');

                    const a = document.createElement('a');
                    a.classList.add('page-link');
                    a.href = `#`; // Puedes cambiar esto para hacer solicitudes de paginación también
                    a.textContent = i;
                    a.onclick = (e) => {
                        e.preventDefault();
                        fetch(`/Juegos/Search?query=${encodeURIComponent(valorBuscar)}&pagina=${i}`)
                            .then(response => response.text())
                            .then(html => {
                                contenedorJuegos.innerHTML = html;
                            });
                    };

                    li.appendChild(a);
                    paginacion.appendChild(li);
                }
            });
    });
});
