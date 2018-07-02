using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Geth;
using Nethereum.JsonRpc.Client;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3.Accounts.Managed;

namespace mediacionAPI.Controllers
{
    [Route("api/[controller]")]
    public class MediacionController : Controller
    {
        string mediadorAddress = "0xfbedef61881d06f16b06c720280392f7c9e4dc05";  //clique
        //string mediadorAddress = "0x369d6Ecb1E319877dc5Dc8633Fc6EfA453423858";   //kaleido
        string cjaAddress = "0xbee40e8faacd42298ddee9e4b93be85619812bea"; 
        string password = "Kiki32";   //clique
        //string password = "xt7u8OJu_88nAEvOeZ1wHqyT3Xz1a2cBuw0RRWQ3mL0";   //kaleido

        string abi = @"[{'constant':false,'inputs':[{'name':'descripcion','type':'bytes32'},{'name':'idMediacion','type':'uint32'},{'name':'ipfsHash','type':'bytes32'},{'name':'oficinaCJA','type':'address'}],'name':'creaNuevaMediacion','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'},{'name':'','type':'uint256'}],'name':'mediaciones','outputs':[{'name':'mediacionId','type':'uint256'},{'name':'terminada','type':'bool'},{'name':'nota','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'idMediacion','type':'uint32'},{'name':'descripcion','type':'bytes32'},{'name':'ipfsHash','type':'bytes32'}],'name':'agregaDocumento','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'cja','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'mediador','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'idMediacion','type':'uint32'}],'name':'agregaInvitado','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'anonymous':false,'inputs':[{'indexed':false,'name':'mediador','type':'address'},{'indexed':false,'name':'idMediacion','type':'uint256'},{'indexed':false,'name':'tipoDocto','type':'bytes32'},{'indexed':false,'name':'participante','type':'bytes32'}],'name':'SeCreoNuevaMediacion','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'mediador','type':'address'},{'indexed':false,'name':'idMediacion','type':'uint256'},{'indexed':false,'name':'tipoDocto','type':'bytes32'}],'name':'SeCreoNuevoDocumento','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'mediador','type':'address'},{'indexed':false,'name':'idMediacion','type':'uint256'},{'indexed':false,'name':'participante','type':'bytes32'}],'name':'SeCreoNuevoParticipante','type':'event'}]";
        string bytecode = "608060405234801561001057600080fd5b50610ba4806100206000396000f3006080604052600436106100775763ffffffff7c0100000000000000000000000000000000000000000000000000000000600035041663221f769c811461007c57806350394ac2146100be5780635ca0b8731461016c57806360242aa114610192578063a350f151146101c3578063dac1d7a7146101d8575b600080fd5b34801561008857600080fd5b506100ac60043563ffffffff60243516604435600160a060020a03606435166101f6565b60408051918252519081900360200190f35b3480156100ca57600080fd5b506100e2600160a060020a0360043516602435610527565b604051808481526020018315151515815260200180602001828103825283818151815260200191508051906020019080838360005b8381101561012f578181015183820152602001610117565b50505050905090810190601f16801561015c5780820380516001836020036101000a031916815260200191505b5094505050505060405180910390f35b34801561017857600080fd5b5061019063ffffffff600435166024356044356105f8565b005b34801561019e57600080fd5b506101a7610759565b60408051600160a060020a039092168252519081900360200190f35b3480156101cf57600080fd5b506101a7610768565b3480156101e457600080fd5b5061019063ffffffff60043516610777565b600061020061091e565b61020861091e565b60008054600160a060020a0333811673ffffffffffffffffffffffffffffffffffffffff199283161783556001805491881691909216179055604080518082018252898152602081018890528151808301909252935090819081526020016000905263ffffffff87166002908155600380546001818101835560009283528651919093027fc2575a0e9e593c00f959f8c92f12db2869c3395a3b0502d05e2516446f71f85b81019190915560208601517fc2575a0e9e593c00f959f8c92f12db2869c3395a3b0502d05e2516446f71f85c90910155600480548084018083559190925283517f8a35acfbc15ff81a39ae7d344fd709f28e8600b4aa8c65c6b64bfe7fe36bd19b909201805494955090938593919291839160ff191690838181111561032f57fe5b021790555060208201518154829061ff00191661010083600281111561035157fe5b0217905550506005805460ff1916905550506040805160608101825260298082527f45737461206573206c61206e6f74612061736f63696164612061206c61206d65602083019081527f6469616369c3b36e2e000000000000000000000000000000000000000000000092909301919091526103cf91600691610935565b5060008054600160a060020a03168152600760209081526040822080546001818101808455928552929093206002805460059095029091019384556003805492949193919261042192840191906109b3565b50600282810180546104369284019190610a0f565b50600382810154908201805460ff191660ff90921615159190911790556004808301805461047a928401919060026101006001831615026000190190911604610aa7565b505060008054600160a060020a0390811682526007602090815260409283902054835133939093168352908201528082018b90527f536f6c69636974616e7465000000000000000000000000000000000000000000606082015290517fd1eb6147bc71fdb7f34982097818afc996ebe46974593a05375544f811e4a76f9350908190036080019150a1505060008054600160a060020a031681526007602052604090205495945050505050565b60076020528160005260406000208181548110151561054257fe5b60009182526020918290206005909102018054600382015460048301805460408051601f6002600019600186161561010002019094169390930492830188900488028101880190915281815293975060ff909216955092939192918301828280156105ee5780601f106105c3576101008083540402835291602001916105ee565b820191906000526020600020905b8154815290600101906020018083116105d157829003601f168201915b5050505050905083565b60005433600160a060020a039081169116141580610625575060015433600160a060020a03908116911614155b151561069257604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601560248201527f4f706572616369c3b36e20696e76c3a16c6964612e0000000000000000000000604482015290519081900360640190fd5b600160a060020a0333166000908152600760205260409020805463ffffffff85169081106106bc57fe5b6000918252602080832060408051808201825287815280840187815260016005909602909301850180548087018255908752958490209051600290960201948555905193909201929092558051600160a060020a033316815263ffffffff861692810192909252818101849052517fdaca70434c366c31fb70c649895c3db78ef439b116ab21ba622743a459daa32a9181900360600190a1505050565b600154600160a060020a031681565b600054600160a060020a031681565b61077f61091e565b60005433600160a060020a03908116911614156107fd57604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601560248201527f4f706572616369c3b36e20696e76c3a16c6964612e0000000000000000000000604482015290519081900360640190fd5b50604080518082018252600181526000602080830182905233600160a060020a031682526007905291909120805463ffffffff841690811061083b57fe5b6000918252602080832060026005909302019190910180546001818101808455928552929093208451930180549193859391929091839160ff191690838181111561088257fe5b021790555060208201518154829061ff0019166101008360028111156108a457fe5b02179055505060408051600160a060020a033316815263ffffffff861660208201527f496e76697461646f0000000000000000000000000000000000000000000000008183015290517f4ffaacc845fb2b428a42a513b6cd21dcf770f9a0a8e697f7351ede2c4b39dae69350908190036060019150a15050565b604080518082019091526000808252602082015290565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f1061097657805160ff19168380011785556109a3565b828001600101855582156109a3579182015b828111156109a3578251825591602001919060010190610988565b506109af929150610b1c565b5090565b828054828255906000526020600020906002028101928215610a035760005260206000209160020282015b82811115610a03578254825560018084015490830155600292830192909101906109de565b506109af929150610b39565b828054828255906000526020600020908101928215610a9b5760005260206000209182015b82811115610a9b57825482548491849160ff90911690829060ff191660018381811115610a5d57fe5b02179055508154815460ff610100928390041691839161ff00191690836002811115610a8557fe5b0217905550505091600101919060010190610a34565b506109af929150610b59565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f10610ae057805485556109a3565b828001600101855582156109a357600052602060002091601f016020900482015b828111156109a3578254825591600101919060010190610b01565b610b3691905b808211156109af5760008155600101610b22565b90565b610b3691905b808211156109af5760008082556001820155600201610b3f565b610b3691905b808211156109af57805461ffff19168155600101610b5f5600a165627a7a723058204b4ca9d1d6a4f0aea9c5b28b588824f2c3b4ed7a1d590658f367ec113154b7c20029";

        //Nethereum.Web3.Web3 geth;
        Web3Geth geth;

        ManagedAccount account;
        string contractAddress = string.Empty;
        /*
        http://localhost:8501
        http://localhost:8502
        */

        //Obtenen Token de Mediación
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //Crea Token de Mediación
        [HttpPost]
        //public async IEnumerable<string> Post()
        public async Task<IActionResult> Post(int idMediacion, [FromBody]JArray documentos)
        {
            try 
            {
                await VerificaExistenciaSmartContract();
                var contract = geth.Eth.GetContract(abi, contractAddress);

                var creaNuevaMediacionFunction = contract.GetFunction("creaNuevaMediacion");
                var mediacionStruct = contract.GetFunction("mediaciones");
                var seCreoNuevaMediacionEvent = contract.GetEvent("SeCreoNuevaMediacion");
                //var mediaciones = contract.GetFunction("mediaciones");

                foreach (JObject item in documentos)
                {
                    string descripcion = item.GetValue("descripcion").ToString();
                    string ipfsHash = item.GetValue("ipfsHash").ToString();

                    var transactionHash = await creaNuevaMediacionFunction.SendTransactionAsync(mediadorAddress, new HexBigInteger(900000), null, descripcion, idMediacion, ipfsHash, cjaAddress);

                    var receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                    while (receipt == null){
                        Thread.Sleep(5000);
                        receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                    }
                }

                var result0 = await mediacionStruct.CallDeserializingToObjectAsync<Mediacion>(mediadorAddress, 0);
                var result1 = await mediacionStruct.CallDeserializingToObjectAsync<Mediacion>(mediadorAddress, 1);
                var filterEvents = await seCreoNuevaMediacionEvent.CreateFilterAsync();
                var logEvents = await seCreoNuevaMediacionEvent.GetFilterChanges<SeCreoNuevaMediacionEvent>(filterEvents);
            }
            catch (Exception chingadamadre)
            {
                Console.WriteLine("Message: " + chingadamadre.Message);
                Console.WriteLine("InnerException: " + chingadamadre.InnerException);
                Console.WriteLine("Source: " + chingadamadre.Source);
                return BadRequest(error: chingadamadre.Message);  //400
            }

            //return "Ready";
            //return Ok(new string[] { "value1", "value2" });
            return Ok("Se ha creado una nueva Mediacion");
        }

        //Actualiza Token de Mediciación
        [HttpPut]
        public async Task<IActionResult> Put(int idMediacion, [FromBody]JArray documentos)
        {
            try
            {
                foreach (JObject item in documentos)
                {
                    string descripcion = item.GetValue("descripcion").ToString();
                    string ipfsHash = item.GetValue("ipfsHash").ToString();
                }
                await VerificaExistenciaSmartContract();
            }
            catch (Exception chingadamadre)
            {
                Console.WriteLine("Message: " + chingadamadre.Message);
                Console.WriteLine("InnerException: " + chingadamadre.InnerException);
                Console.WriteLine("Source: " + chingadamadre.Source);
                return BadRequest(error: chingadamadre.Message);  //400
            }

            //return new string[] { "value1", "value2" };
            return Ok("Se han agregados nuevos documentos a la Mediacion " + idMediacion.ToString());
        }

        private async Task VerificaExistenciaSmartContract() 
        {
            if (contractAddress == string.Empty)
            {
                account = new ManagedAccount(mediadorAddress, password);  //Kaleido + Clique
                geth = new Web3Geth(account, "http://localhost:8501");  //Clique
                try
                {
                    /**** Kaleido ****/
                    /*var byteArray = Encoding.ASCII.GetBytes("u0n94z8hht:xt7u8OJu_88nAEvOeZ1wHqyT3Xz1a2cBuw0RRWQ3mL0");
                    AuthenticationHeaderValue autenticacion = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var client = new RpcClient(new Uri("https://u0yaaa1f6n-u0s4p500nd-rpc.us-east-2.kaleido.io"),autenticacion);
                    var geth = new Web3(client);*/
                    /*****************/

                    /****** Clique *****/
                    var unlockAccountResult = await geth.Personal.UnlockAccount.SendRequestAsync(cjaAddress, password, 120);
                    /*****************/
                    
                    var transactionHash = await geth.Eth.DeployContract.SendRequestAsync(abi, bytecode, mediadorAddress, new HexBigInteger(900000));
                    var receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                    while (receipt == null)
                    {
                        Thread.Sleep(5000);
                        receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                    }
                    contractAddress = receipt.ContractAddress;
                }
                catch (Exception chingadamadre)
                {
                    Console.WriteLine("Message: " + chingadamadre.Message);
                    Console.WriteLine("InnerException: " + chingadamadre.InnerException);
                    Console.WriteLine("Source: " + chingadamadre.Source);
                }
            }
        }
    }

    //event SeCreoNuevaMediacion(address mediador, uint idMediacion, bytes32 tipoDocto, bytes32 participante);
    [FunctionOutput]
    public class SeCreoNuevaMediacionEvent
    {
        [Parameter("address", "mediador", 1)]
        public string Mediador {get; set;}

        [Parameter("int", "idMediacion", 2)]
        public int IdMediacion {get; set;}

        [Parameter("string", "tipoDocto", 3)]
        public string TipoDocto {get; set;}

        [Parameter("string", "participante", 4)]
        public string Participante {get; set;}
    }

    /*
    struct Mediacion {
        uint256 mediacionId;
        Documento[] documentos;
        Participante[] participantes;
        bool terminada;
        string nota;
    }
     */
    [FunctionOutput]
    public class Mediacion
    {
        //[Parameter("int", "mediacionId", 1)]
        public int MediacionId {get; set;}

        //[Parameter("Object[]", "documentos", 2)]
        public Object[] Documentos {get; set;}

        //[Parameter("Object[]", "participantes", 3)]
        public Object[] participantes {get; set;}

        //[Parameter("bool", "terminada", 4)]
        public bool terminada {get; set;}

        //[Parameter("string", "nota", 4)]
        public string nota {get; set;}
    }
}