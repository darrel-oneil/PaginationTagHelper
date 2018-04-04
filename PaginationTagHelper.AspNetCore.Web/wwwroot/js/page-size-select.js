// --------------------------------------------------------------
// Listen to change event on the pageSize select and submit form.
// --------------------------------------------------------------
var pageSizeSelect = document.querySelector('#pageSize');
if (pageSizeSelect) {
    pageSizeSelect.addEventListener('change', function () {
        this.form.submit();
    });
}
else {
    console.error("Could not locate 'pageSize' select element.");
}