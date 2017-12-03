/**
 * 
 */

function returnValuesCliente(cod, nom, ape, tipo, num, venc, codseg, msg) {

    var msg = document.getElementById("idMsg").value;

    if (msg != "" && msg.includes("buscar")) {

        window.opener.document.getElementById("idCodCli").value = cod;
        window.opener.document.getElementById("idNomCli").value = nom + ' ' + ape;

        if (msg == "buscar") {
            window.opener.document.getElementById("idTipoTarj").value = tipo;
            window.opener.document.getElementById("idNumTarj").value = num;
            window.opener.document.getElementById("idFecVenc").value = venc;
            window.opener.document.getElementById("idCodSeg").value = codseg;
        }

        window.close();
    }
}


function resetFecha() {
    document.getElementById("idFecha").value = "";
}


function validarDatosListaHab() {

    var fecha = document.getElementById("idFecha").value;

    if (fecha == "") {
        alert('[ERROR] Campo Fecha es obligatorio');
        return false;
    }

    return true;
}


function validarDatosListaCli() {

    var codi = document.getElementById("idCod").value;
    var apel = document.getElementById("idApe").value;
    var docu = document.getElementById("idDoc").value;
    var c = 0;

    if (codi.trim().length > 0)
        c = c + 1;

    if (apel.trim().length > 0)
        c = c + 1;

    if (docu.trim().length > 0)
        c = c + 1;

    if (c != 1) {
        alert('[ERROR] Debe ingresar datos en un solo campo');
        return false;
    }

    return true;
}


function validarDatosListaRes() {

    var codi = document.getElementById("idCod").value;
    var apel = document.getElementById("idApe").value;
    var docu = document.getElementById("idDoc").value;
    var fech = document.getElementById("idFecha").value;
    var c = 0;

    if (codi.trim().length > 0)
        c = c + 1;

    if (apel.trim().length > 0)
        c = c + 1;

    if (docu.trim().length > 0)
        c = c + 1;

    if (fech.trim().length > 0)
        c = c + 1;

    if (c != 1) {
        alert('[ERROR] Debe ingresar datos en un solo campo');
        return false;
    }

    return true;
}


function validarDatosCreaEmp() {

    var fecha1 = document.getElementById("idFecNac").value;
    var fecha2 = document.getElementById("idFecCon").value;

    if (fecha1 == "" || fecha2 == "") {
        alert('[ERROR] Campos de fecha son obligatorios');
        return false;
    }

    return true;
}


function validarDatosClaves() {

    var clave1 = document.getElementById("idClave1").value;
    var clave2 = document.getElementById("idClave2").value;

    if (clave1 !== clave2) {
        alert('[ERROR] Las nuevas claves ingresadas son diferentes');
        return false;
    }
    return true;
}
