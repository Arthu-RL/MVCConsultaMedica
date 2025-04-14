// Ativa os tooltips do Bootstrap
document.addEventListener('DOMContentLoaded', function () {
  var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
  tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
  });

  // Adiciona classe 'active' ao item atual do menu
  highlightCurrentNavItem();

  // Formata valores monetários na tabela
  formatCurrencyValues();
});

function highlightCurrentNavItem() {
  const currentPath = window.location.pathname;
  const navLinks = document.querySelectorAll('.navbar-nav .nav-link');

  navLinks.forEach(link => {
    if (link.getAttribute('href') === currentPath) {
      link.classList.add('active');
    }
  });
}

function formatCurrencyValues() {
  const currencyElements = document.querySelectorAll('.currency-value');

  currencyElements.forEach(element => {
    const value = parseFloat(element.textContent);
    if (!isNaN(value)) {
      element.textContent = value.toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      });
    }
  });
}
