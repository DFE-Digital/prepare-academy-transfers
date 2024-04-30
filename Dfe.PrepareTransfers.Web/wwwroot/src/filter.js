document.addEventListener('DOMContentLoaded', function () {
   // If this method is called JS is enabled, thus boxes should be shown
   showFilters();
   setupFilters();

   function setupFilters() {
      const filterSections = document.querySelectorAll('.govuk-accordion__section');
      filterSections.forEach(section => {
         const searchInput = section.querySelector('.govuk-input');
         const items = section.querySelectorAll('.govuk-checkboxes__item');

         if (searchInput) {
            searchInput.addEventListener('keyup', function (e) {
               const searchValue = e.target.value.toLowerCase();
               filterItems(items, searchValue);
            });
         }
      });
   }
   function showFilters() {
      // Select all input elements with the 'govuk-!-display-none' class
      const inputs = document.querySelectorAll('input.govuk-\\!-display-none');

      // Iterate over each input and remove the 'govuk-!-display-none' class to display them
      inputs.forEach(function (input) {
         input.classList.remove('govuk-!-display-none');
      });
   }

   function filterItems(items, searchValue) {
      items.forEach(function (item) {
         const itemName = item.textContent.toLowerCase();
         if (itemName.includes(searchValue)) {
            item.style.display = '';
         } else {
            item.style.display = 'none';
         }
      });
   }
});
