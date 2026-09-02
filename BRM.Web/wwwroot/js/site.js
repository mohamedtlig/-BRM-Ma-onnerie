// Adaptateur de validation client pour les cases à cocher obligatoires (RGPD, etc.),
// utilisé avec l'attribut serveur MustBeTrueAttribute. Le validateur jQuery "range" natif
// ne fonctionne pas correctement sur les booléens : "mustbetrue" le remplace.
$(function () {
    if (window.jQuery && $.validator && $.validator.unobtrusive) {
        $.validator.addMethod("mustbetrue", function (value, element) {
            return element.checked === true;
        });
        $.validator.unobtrusive.adapters.addBool("mustbetrue");
    }
});
