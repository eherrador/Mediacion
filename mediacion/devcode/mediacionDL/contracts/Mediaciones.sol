pragma solidity ^0.4.23;

contract Mediaciones {
    address public mediador;
    //address public solicitante;
    //address public invitado;
    address public cja;

    enum TiposDocumentos { Invitacion, ReglasConduccion, LibreVoluntadInvitado, LibreVoluntadSolicitante, ConvenioConfidencialidad, 
    AceptacionServicioMediacion, EscritoAutonomia, TarjetaInformativa, EtapasMediacion, ConstruccionSoluciones, ConvenioMediacion }
    struct Documento {
        TiposDocumentos tipoDocumento;   // nombre del documento
        bytes32 ipfsHash; // hash del documento almacenado en IPFS
    }

    enum TiposIdentificacionOficial { INE, Pasaporte, FM3 }
    struct IdentificacionOficial {
        TiposIdentificacionOficial tipoIdentificacion; // Credencial del INE, Pasaporte, FM3
        //bytes32 ipfsHash; // hash de la identificación almacenada en IPFS
    }

    enum Roles {Solicitante, Invitado}
    struct Participante {
        //bytes32 fiel;
        Roles rol; //para especificar si es: mediador, solicitante, invitado o es el CJA
        TiposIdentificacionOficial identificacion;
    }

    struct Mediacion {
        uint256 mediacionId;
        Documento[] documentos;
        Participante[] participantes;
        bool terminada;
        string nota;
    }

    Mediacion mediacion;
    mapping(address => Mediacion[]) public mediaciones;

    event SeCreoNuevaMediacion(address mediador, uint256 idMediacion, bytes32 tipoDocto, bytes32 participante);
    event SeCreoNuevoDocumento(address mediador, uint256 idMediacion, bytes32 tipoDocto);
    event SeCreoNuevoParticipante(address mediador, uint256 idMediacion, bytes32 participante);

    function creaNuevaMediacion(bytes32 ipfsHash, address oficinaCJA) public returns (uint256)
    {
        mediador = msg.sender;
        cja = oficinaCJA;

        Documento memory documento = Documento ({tipoDocumento: TiposDocumentos.ReglasConduccion, ipfsHash: ipfsHash});
        Participante memory participante = Participante ({rol: Roles.Solicitante, identificacion: TiposIdentificacionOficial.INE});

        mediacion.mediacionId = mediaciones[mediador].length + 1;
        mediacion.documentos.push(documento);
        mediacion.participantes.push(participante);
        mediacion.terminada = false;
        mediacion.nota = "";
        
        mediaciones[mediador].push(mediacion);

        emit SeCreoNuevaMediacion(msg.sender, mediaciones[mediador].length, "Reglas Conducción", "Solicitante");

        return mediaciones[mediador].length;
    }

    function agregaInvitado(uint32 idMediacion) public
    {
        require(msg.sender != mediador, "Operación inválida.");

        Participante memory p = Participante ({rol: Roles.Invitado, identificacion: TiposIdentificacionOficial.INE});
        mediaciones[msg.sender][idMediacion].participantes.push(p);

        emit SeCreoNuevoParticipante(msg.sender, idMediacion, "Invitado");
    }

    function agregaDocumento(uint32 idMediacion, uint tipoDocto, bytes32 ipfsHash) public 
    {
        require(msg.sender != mediador || msg.sender != cja, "Operación inválida.");

        bytes32 doctoType;

        if (tipoDocto == 1) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.Invitacion, ipfsHash: ipfsHash}));
            doctoType = "Invitación";
        }
        if (tipoDocto == 2) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.ReglasConduccion, ipfsHash: ipfsHash}));
            doctoType = "Reglas Conducción";
        }
        if (tipoDocto == 3) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.LibreVoluntadInvitado, ipfsHash: ipfsHash}));
            doctoType = "Libre Voluntad Invitado";
        }
        if (tipoDocto == 4) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.LibreVoluntadSolicitante, ipfsHash: ipfsHash}));
            doctoType = "Libre Voluntad Solicitante";
        }
        if (tipoDocto == 5) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.ConvenioConfidencialidad, ipfsHash: ipfsHash}));
            doctoType = "Convenio Confidencialidad";
        }
        if (tipoDocto == 6) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.AceptacionServicioMediacion, ipfsHash: ipfsHash}));
            doctoType = "Aceptacion Servicio Mediacion";
        }
        if (tipoDocto == 7) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.EscritoAutonomia, ipfsHash: ipfsHash}));
            doctoType = "Escrito Autonomía";
        }
        if (tipoDocto == 8) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.TarjetaInformativa, ipfsHash: ipfsHash}));
            doctoType = "Tarjeta Informativa";
        }
        if (tipoDocto == 9) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.EtapasMediacion, ipfsHash: ipfsHash}));
            doctoType = "Etapas Mediación";
        }
        if (tipoDocto == 10) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.ConstruccionSoluciones, ipfsHash: ipfsHash}));
            doctoType = "Construcción Soluciones";
        }
        if (tipoDocto == 11) {
            mediaciones[msg.sender][idMediacion].documentos.push(Documento({tipoDocumento: TiposDocumentos.ConvenioMediacion, ipfsHash: ipfsHash}));
            doctoType = "Convenio Mediación";
        }

        emit SeCreoNuevoDocumento(msg.sender, idMediacion, doctoType);
    }
}