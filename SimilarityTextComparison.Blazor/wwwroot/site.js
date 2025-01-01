window.PrintJs = {
    printElement: function (elementId) {
        var printContents = document.getElementById(elementId).innerHTML;
        var originalContents = document.body.innerHTML;

        document.body.innerHTML = printContents;

        window.print();

        document.body.innerHTML = originalContents;
        window.location.reload(); // Ricarica la pagina per ripristinare lo stato
    }
};