using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
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
        string mediadorAddress = "0xfbedef61881d06f16b06c720280392f7c9e4dc05";
        string cjaAddress = "0xbee40e8faacd42298ddee9e4b93be85619812bea"; 
        string password = "Kiki32";
        string abi = @"[{'constant':false,'inputs':[{'name':'idMediacion','type':'uint32'},{'name':'tipoDocto','type':'uint256'},{'name':'ipfsHash','type':'bytes32'}],'name':'agregaDocumento','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'},{'name':'','type':'uint256'}],'name':'mediaciones','outputs':[{'name':'mediacionId','type':'uint256'},{'name':'terminada','type':'bool'},{'name':'nota','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'cja','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'mediador','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'idMediacion','type':'uint32'}],'name':'agregaInvitado','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'ipfsHash','type':'bytes32'},{'name':'oficinaCJA','type':'address'}],'name':'creaNuevaMediacion','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'anonymous':false,'inputs':[{'indexed':false,'name':'mediador','type':'address'},{'indexed':false,'name':'idMediacion','type':'uint256'},{'indexed':false,'name':'tipoDocto','type':'bytes32'},{'indexed':false,'name':'participante','type':'bytes32'}],'name':'SeCreoNuevaMediacion','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'mediador','type':'address'},{'indexed':false,'name':'idMediacion','type':'uint256'},{'indexed':false,'name':'tipoDocto','type':'bytes32'}],'name':'SeCreoNuevoDocumento','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'mediador','type':'address'},{'indexed':false,'name':'idMediacion','type':'uint256'},{'indexed':false,'name':'participante','type':'bytes32'}],'name':'SeCreoNuevoParticipante','type':'event'}]";
        string bytecode = "608060405234801561001057600080fd5b50611436806100206000396000f3006080604052600436106100775763ffffffff7c01000000000000000000000000000000000000000000000000000000006000350416630aeb111b811461007c57806350394ac2146100a257806360242aa114610150578063a350f15114610181578063dac1d7a714610196578063f08f0132146101b4575b600080fd5b34801561008857600080fd5b506100a063ffffffff600435166024356044356101ea565b005b3480156100ae57600080fd5b506100c6600160a060020a0360043516602435610be9565b604051808481526020018315151515815260200180602001828103825283818151815260200191508051906020019080838360005b838110156101135781810151838201526020016100fb565b50505050905090810190601f1680156101405780820380516001836020036101000a031916815260200191505b5094505050505060405180910390f35b34801561015c57600080fd5b50610165610cba565b60408051600160a060020a039092168252519081900360200190f35b34801561018d57600080fd5b50610165610cc9565b3480156101a257600080fd5b506100a063ffffffff60043516610cd8565b3480156101c057600080fd5b506101d8600435600160a060020a0360243516610e7f565b60408051918252519081900360200190f35b6000805433600160a060020a039081169116141580610218575060015433600160a060020a03908116911614155b151561028557604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601560248201527f4f706572616369c3b36e20696e76c3a16c6964612e0000000000000000000000604482015290519081900360640190fd5b826001141561035857600160a060020a0333166000908152600760205260409020805463ffffffff86169081106102b857fe5b906000526020600020906005020160010160408051908101604052806000600a8111156102e157fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a81111561032157fe5b021790555060209190910151600190910155507f496e766974616369c3b36e00000000000000000000000000000000000000000090505b826002141561042b57600160a060020a0333166000908152600760205260409020805463ffffffff861690811061038b57fe5b906000526020600020906005020160010160408051908101604052806001600a8111156103b457fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a8111156103f457fe5b021790555060209190910151600190910155507f5265676c617320436f6e6475636369c3b36e000000000000000000000000000090505b82600314156104fe57600160a060020a0333166000908152600760205260409020805463ffffffff861690811061045e57fe5b906000526020600020906005020160010160408051908101604052806002600a81111561048757fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a8111156104c757fe5b021790555060209190910151600190910155507f4c6962726520566f6c756e74616420496e76697461646f00000000000000000090505b82600414156105d157600160a060020a0333166000908152600760205260409020805463ffffffff861690811061053157fe5b906000526020600020906005020160010160408051908101604052806003600a81111561055a57fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a81111561059a57fe5b021790555060209190910151600190910155507f4c6962726520566f6c756e74616420536f6c69636974616e746500000000000090505b82600514156106a457600160a060020a0333166000908152600760205260409020805463ffffffff861690811061060457fe5b906000526020600020906005020160010160408051908101604052806004600a81111561062d57fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a81111561066d57fe5b021790555060209190910151600190910155507f436f6e76656e696f20436f6e666964656e6369616c696461640000000000000090505b826006141561077757600160a060020a0333166000908152600760205260409020805463ffffffff86169081106106d757fe5b906000526020600020906005020160010160408051908101604052806005600a81111561070057fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a81111561074057fe5b021790555060209190910151600190910155507f41636570746163696f6e20536572766963696f204d6564696163696f6e00000090505b826007141561084a57600160a060020a0333166000908152600760205260409020805463ffffffff86169081106107aa57fe5b906000526020600020906005020160010160408051908101604052806006600a8111156107d357fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a81111561081357fe5b021790555060209190910151600190910155507f4573637269746f204175746f6e6f6dc3ad61000000000000000000000000000090505b826008141561091d57600160a060020a0333166000908152600760205260409020805463ffffffff861690811061087d57fe5b906000526020600020906005020160010160408051908101604052806007600a8111156108a657fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a8111156108e657fe5b021790555060209190910151600190910155507f5461726a65746120496e666f726d61746976610000000000000000000000000090505b82600914156109f057600160a060020a0333166000908152600760205260409020805463ffffffff861690811061095057fe5b906000526020600020906005020160010160408051908101604052806008600a81111561097957fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a8111156109b957fe5b021790555060209190910151600190910155507f457461706173204d656469616369c3b36e00000000000000000000000000000090505b82600a1415610ac357600160a060020a0333166000908152600760205260409020805463ffffffff8616908110610a2357fe5b906000526020600020906005020160010160408051908101604052806009600a811115610a4c57fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a811115610a8c57fe5b021790555060209190910151600190910155507f436f6e73747275636369c3b36e20536f6c7563696f6e6573000000000000000090505b82600b1415610b9557600160a060020a0333166000908152600760205260409020805463ffffffff8616908110610af657fe5b90600052602060002090600502016001016040805190810160405280600a80811115610b1e57fe5b81526020908101859052825460018181018086556000958652929094208351600290920201805492949092839160ff199091169083600a811115610b5e57fe5b021790555060209190910151600190910155507f436f6e76656e696f204d656469616369c3b36e0000000000000000000000000090505b60408051600160a060020a033316815263ffffffff8616602082015280820183905290517fdaca70434c366c31fb70c649895c3db78ef439b116ab21ba622743a459daa32a9181900360600190a150505050565b600760205281600052604060002081815481101515610c0457fe5b60009182526020918290206005909102018054600382015460048301805460408051601f6002600019600186161561010002019094169390930492830188900488028101880190915281815293975060ff90921695509293919291830182828015610cb05780601f10610c8557610100808354040283529160200191610cb0565b820191906000526020600020905b815481529060010190602001808311610c9357829003601f168201915b5050505050905083565b600154600160a060020a031681565b600054600160a060020a031681565b610ce0611188565b60005433600160a060020a0390811691161415610d5e57604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601560248201527f4f706572616369c3b36e20696e76c3a16c6964612e0000000000000000000000604482015290519081900360640190fd5b50604080518082018252600181526000602080830182905233600160a060020a031682526007905291909120805463ffffffff8416908110610d9c57fe5b6000918252602080832060026005909302019190910180546001818101808455928552929093208451930180549193859391929091839160ff1916908381811115610de357fe5b021790555060208201518154829061ff001916610100836002811115610e0557fe5b02179055505060408051600160a060020a033316815263ffffffff861660208201527f496e76697461646f0000000000000000000000000000000000000000000000008183015290517f4ffaacc845fb2b428a42a513b6cd21dcf770f9a0a8e697f7351ede2c4b39dae69350908190036060019150a15050565b6000610e89611188565b610e91611188565b505060008054600160a060020a0333811673ffffffffffffffffffffffffffffffffffffffff199283161780845560018054878416941693909317835560408051808201825284815260208082018a90528251808401845287815280820188905293909416865260079093528420548301600290815560038054808601808355919096528351959091027fc2575a0e9e593c00f959f8c92f12db2869c3395a3b0502d05e2516446f71f85b01805493959294919386939192839160ff19169083600a811115610f5c57fe5b0217905550602091909101516001918201556004805480830180835560009290925284517f8a35acfbc15ff81a39ae7d344fd709f28e8600b4aa8c65c6b64bfe7fe36bd19b909101805492945085939092839160ff19909116908381811115610fc157fe5b021790555060208201518154829061ff001916610100836002811115610fe357fe5b0217905550506005805460ff1916905550506040805160208101918290526000908190526110139160069161119f565b5060008054600160a060020a031681526007602090815260408220805460018181018084559285529290932060028054600590950290910193845560038054929491939192611065928401919061121d565b506002828101805461107a928401919061129c565b50600382810154908201805460ff191660ff9092161515919091179055600480830180546110be928401919060026101006001831615026000190190911604611334565b505060008054600160a060020a0390811682526007602090815260409283902054835133939093168352908201527f5265676c617320436f6e6475636369c3b36e0000000000000000000000000000818301527f536f6c69636974616e7465000000000000000000000000000000000000000000606082015290517fd1eb6147bc71fdb7f34982097818afc996ebe46974593a05375544f811e4a76f9350908190036080019150a1505060008054600160a060020a03168152600760205260409020549392505050565b604080518082019091526000808252602082015290565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f106111e057805160ff191683800117855561120d565b8280016001018555821561120d579182015b8281111561120d5782518255916020019190600101906111f2565b506112199291506113a9565b5090565b8280548282559060005260206000209060020281019282156112905760005260206000209160020282015b8281111561129057825482548491849160ff90911690829060ff1916600183600a81111561127257fe5b02179055506001918201549101556002928301929190910190611248565b506112199291506113c6565b8280548282559060005260206000209081019282156113285760005260206000209182015b8281111561132857825482548491849160ff90911690829060ff1916600183818111156112ea57fe5b02179055508154815460ff610100928390041691839161ff0019169083600281111561131257fe5b02179055505050916001019190600101906112c1565b506112199291506113eb565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f1061136d578054855561120d565b8280016001018555821561120d57600052602060002091601f016020900482015b8281111561120d57825482559160010191906001019061138e565b6113c391905b8082111561121957600081556001016113af565b90565b6113c391905b8082111561121957805460ff19168155600060018201556002016113cc565b6113c391905b8082111561121957805461ffff191681556001016113f15600a165627a7a7230582057dd9b25337c76d86c4bdc74d5f03737b1887ba12eed3c28ce43dbf33d1203840029";
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
        public async Task<IActionResult> Post()
        {
            await VerificaExistenciaSmartContract();
            var contract = geth.Eth.GetContract(abi, contractAddress);

            var creaNuevaMediacionFunction = contract.GetFunction("creaNuevaMediacion");
            var mediacionStruct = contract.GetFunction("mediaciones");
            var seCreoNuevaMediacionEvent = contract.GetEvent("SeCreoNuevaMediacion");
            //var mediaciones = contract.GetFunction("mediaciones");

            var transactionHash = await creaNuevaMediacionFunction.SendTransactionAsync(mediadorAddress, new HexBigInteger(900000), null, "123qwe456asd", cjaAddress);

            var receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            while (receipt == null){
                Thread.Sleep(5000);
                receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }

            var result0 = await mediacionStruct.CallDeserializingToObjectAsync<Mediacion>(mediadorAddress, 0);
            var result1 = await mediacionStruct.CallDeserializingToObjectAsync<Mediacion>(mediadorAddress, 1);
            var filterEvents = await seCreoNuevaMediacionEvent.CreateFilterAsync();
            var logEvents = await seCreoNuevaMediacionEvent.GetFilterChanges<SeCreoNuevaMediacionEvent>(filterEvents);

            //return "Ready";
            return Ok(new string[] { "value1", "value2" });
        }

        //Actualiza Token de Mediciación
        [HttpPut]
        public IEnumerable<string> Put()
        {
            return new string[] { "value1", "value2" };
        }

        private async Task VerificaExistenciaSmartContract() 
        {
            if (contractAddress == string.Empty)
            {
                //a managed account uses personal_sendTransanction with the given password,
                //this way we don't need to unlock the account for a certain period of time
                account = new ManagedAccount(mediadorAddress, password);
                geth = new Web3Geth(account, "http://localhost:8501");
                //geth = new Web3Geth("http://localhost:8501");
                try
                {
                    //var unlockAccountResult = await geth.Personal.UnlockAccount.SendRequestAsync(cjaAddress, password, 120);
                    var transactionHash = await geth.Eth.DeployContract.SendRequestAsync(abi, bytecode, mediadorAddress, new Nethereum.Hex.HexTypes.HexBigInteger(900000));
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