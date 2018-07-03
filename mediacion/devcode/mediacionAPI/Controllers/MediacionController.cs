using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Geth;
using Nethereum.JsonRpc.Client;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.Hex.HexConvertors.Extensions;

namespace mediacionAPI.Controllers
{
    [Route("api/[controller]")]
    public class MediacionController : Controller
    {
        string mediadorAddress   = "0xfbedef61881d06f16b06c720280392f7c9e4dc05";  //clique
        //string mediadorAddress = "0x369d6Ecb1E319877dc5Dc8633Fc6EfA453423858";   //kaleido
        string cjaAddress        = "0xbee40e8faacd42298ddee9e4b93be85619812bea"; 
        string password = "Kiki32";   //clique
        //string password = "xt7u8OJu_88nAEvOeZ1wHqyT3Xz1a2cBuw0RRWQ3mL0";   //kaleido

        string abi = @"[{'constant':false,'inputs':[{'name':'descripcion','type':'bytes32'},{'name':'idMediacion','type':'uint32'},{'name':'ipfsHash','type':'bytes32'}],'name':'creaNuevaMediacion','outputs':[{'name':'','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'},{'name':'','type':'uint256'}],'name':'mediaciones','outputs':[{'name':'mediacionId','type':'uint32'},{'name':'terminada','type':'bool'},{'name':'nota','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'idMediacion','type':'uint32'},{'name':'descripcion','type':'bytes32'},{'name':'ipfsHash','type':'bytes32'}],'name':'agregaDocumento','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'cja','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'mediador','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'idMediacion','type':'uint32'}],'name':'agregaInvitado','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'anonymous':false,'inputs':[{'indexed':true,'name':'mediador','type':'address'},{'indexed':true,'name':'idMediacion','type':'uint32'},{'indexed':true,'name':'tipoDocto','type':'bytes32'},{'indexed':false,'name':'participante','type':'bytes32'}],'name':'SeCreoNuevaMediacion','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'mediador','type':'address'},{'indexed':false,'name':'idMediacion','type':'uint256'},{'indexed':false,'name':'tipoDocto','type':'bytes32'}],'name':'SeCreoNuevoDocumento','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'mediador','type':'address'},{'indexed':false,'name':'idMediacion','type':'uint256'},{'indexed':false,'name':'participante','type':'bytes32'}],'name':'SeCreoNuevoParticipante','type':'event'}]";
        string bytecode = "608060405234801561001057600080fd5b50610b7e806100206000396000f3006080604052600436106100775763ffffffff7c01000000000000000000000000000000000000000000000000000000006000350416632d4eaf57811461007c57806350394ac2146100b45780635ca0b8731461016e57806360242aa114610194578063a350f151146101c5578063dac1d7a7146101da575b600080fd5b34801561008857600080fd5b506100a060043563ffffffff602435166044356101f8565b604080519115158252519081900360200190f35b3480156100c057600080fd5b506100d8600160a060020a03600435166024356104fa565b604051808463ffffffff1663ffffffff1681526020018315151515815260200180602001828103825283818151815260200191508051906020019080838360005b83811015610131578181015183820152602001610119565b50505050905090810190601f16801561015e5780820380516001836020036101000a031916815260200191505b5094505050505060405180910390f35b34801561017a57600080fd5b5061019263ffffffff600435166024356044356105d2565b005b3480156101a057600080fd5b506101a9610733565b60408051600160a060020a039092168252519081900360200190f35b3480156101d157600080fd5b506101a9610742565b3480156101e657600080fd5b5061019263ffffffff60043516610751565b60006102026108f8565b61020a6108f8565b50506000805473ffffffffffffffffffffffffffffffffffffffff191633600160a060020a0316178155604080518082018252868152602080820186815283518085019094528484529083018490526002805463ffffffff191663ffffffff8916178155600380546001818101835591875284517fc2575a0e9e593c00f959f8c92f12db2869c3395a3b0502d05e2516446f71f85b919093029081019290925591517fc2575a0e9e593c00f959f8c92f12db2869c3395a3b0502d05e2516446f71f85c90910155600480548083018083559190955283517f8a35acfbc15ff81a39ae7d344fd709f28e8600b4aa8c65c6b64bfe7fe36bd19b9095018054939591938593919291839160ff191690838181111561032257fe5b021790555060208201518154829061ff00191661010083600281111561034457fe5b0217905550506005805460ff1916905550506040805160608101825260298082527f45737461206573206c61206e6f74612061736f63696164612061206c61206d65602083019081527f6469616369c3b36e2e000000000000000000000000000000000000000000000092909301919091526103c29160069161090f565b5060008054600160a060020a0316815260076020908152604082208054600181810180845592855292909320600280546005909502909101805463ffffffff191663ffffffff90951694909417845560038054929491939192610428928401919061098d565b506002828101805461043d92840191906109e9565b50600382810154908201805460ff191660ff909216151591909117905560048083018054610481928401919060026101006001831615026000190190911604610a81565b5050604080517f536f6c69636974616e74650000000000000000000000000000000000000000008152905189935063ffffffff89169250600160a060020a033316917f776051d613d6412a12cc755c365a92dbec9c6e392ee7caf8de1b3143501a35e7919081900360200190a450600195945050505050565b60076020528160005260406000208181548110151561051557fe5b60009182526020918290206005909102018054600382015460048301805460408051601f6002600019600186161561010002019094169390930492830188900488028101880190915281815263ffffffff909416975060ff909216955092939192918301828280156105c85780601f1061059d576101008083540402835291602001916105c8565b820191906000526020600020905b8154815290600101906020018083116105ab57829003601f168201915b5050505050905083565b60005433600160a060020a0390811691161415806105ff575060015433600160a060020a03908116911614155b151561066c57604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601560248201527f4f706572616369c3b36e20696e76c3a16c6964612e0000000000000000000000604482015290519081900360640190fd5b600160a060020a0333166000908152600760205260409020805463ffffffff851690811061069657fe5b6000918252602080832060408051808201825287815280840187815260016005909602909301850180548087018255908752958490209051600290960201948555905193909201929092558051600160a060020a033316815263ffffffff861692810192909252818101849052517fdaca70434c366c31fb70c649895c3db78ef439b116ab21ba622743a459daa32a9181900360600190a1505050565b600154600160a060020a031681565b600054600160a060020a031681565b6107596108f8565b60005433600160a060020a03908116911614156107d757604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601560248201527f4f706572616369c3b36e20696e76c3a16c6964612e0000000000000000000000604482015290519081900360640190fd5b50604080518082018252600181526000602080830182905233600160a060020a031682526007905291909120805463ffffffff841690811061081557fe5b6000918252602080832060026005909302019190910180546001818101808455928552929093208451930180549193859391929091839160ff191690838181111561085c57fe5b021790555060208201518154829061ff00191661010083600281111561087e57fe5b02179055505060408051600160a060020a033316815263ffffffff861660208201527f496e76697461646f0000000000000000000000000000000000000000000000008183015290517f4ffaacc845fb2b428a42a513b6cd21dcf770f9a0a8e697f7351ede2c4b39dae69350908190036060019150a15050565b604080518082019091526000808252602082015290565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f1061095057805160ff191683800117855561097d565b8280016001018555821561097d579182015b8281111561097d578251825591602001919060010190610962565b50610989929150610af6565b5090565b8280548282559060005260206000209060020281019282156109dd5760005260206000209160020282015b828111156109dd578254825560018084015490830155600292830192909101906109b8565b50610989929150610b13565b828054828255906000526020600020908101928215610a755760005260206000209182015b82811115610a7557825482548491849160ff90911690829060ff191660018381811115610a3757fe5b02179055508154815460ff610100928390041691839161ff00191690836002811115610a5f57fe5b0217905550505091600101919060010190610a0e565b50610989929150610b33565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f10610aba578054855561097d565b8280016001018555821561097d57600052602060002091601f016020900482015b8281111561097d578254825591600101919060010190610adb565b610b1091905b808211156109895760008155600101610afc565b90565b610b1091905b808211156109895760008082556001820155600201610b19565b610b1091905b8082111561098957805461ffff19168155600101610b395600a165627a7a7230582069b293b1763ce955c2701e707eb530671ec8d6a40e9015646269b4ba4ccb6b340029";

        //Web3 geth;  //Kaleido
        Web3Geth geth;   //Clique

        ManagedAccount account;
        
        private SmartContractConfiguration _smartContractConfiguration;

        public MediacionController(IOptions<SmartContractConfiguration> SmartContractConfigurationAccessor)
        {
            _smartContractConfiguration = SmartContractConfigurationAccessor.Value;
        }

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
                CreaAccountyGeth();

                await VerificaExistenciaSmartContract();
                
                var contract = geth.Eth.GetContract(abi, _smartContractConfiguration.SmartContractAddress);

                var creaNuevaMediacionFunction = contract.GetFunction("creaNuevaMediacion");
                var mediacionStruct = contract.GetFunction("mediaciones");
                var seCreoNuevaMediacionEvent = contract.GetEvent("SeCreoNuevaMediacion");

                foreach (JObject item in documentos)
                {
                    string descripcion = item.GetValue("descripcion").ToString();
                    string ipfsHash = item.GetValue("ipfsHash").ToString();

                    cjaAddress = cjaAddress.ToLower().RemoveHexPrefix().PadLeft(40,'0').EnsureHexPrefix();
                    mediadorAddress = mediadorAddress.ToLower().RemoveHexPrefix().PadLeft(40,'0').EnsureHexPrefix();

                    //var filterEvents = await seCreoNuevaMediacionEvent.CreateFilterAsync();

                    var transactionHash = await creaNuevaMediacionFunction.SendTransactionAsync(mediadorAddress, new HexBigInteger(900000), null, descripcion, Convert.ToInt32(idMediacion), ipfsHash);

                    var receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                    while (receipt == null){
                        Thread.Sleep(5000);
                        receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                    }

                    //var logEvents = await seCreoNuevaMediacionEvent.GetFilterChanges<SeCreoNuevaMediacionEvent>(filterEvents);
                }
            }
            catch (Exception chingadamadre)
            {
                Console.WriteLine("Message: " + chingadamadre.Message);
                Console.WriteLine("InnerException: " + chingadamadre.InnerException);
                Console.WriteLine("Source: " + chingadamadre.Source);
                return BadRequest(error: chingadamadre.Message);  //400
            }

            return Ok("Se ha creado una nueva Mediacion con id = " + idMediacion.ToString());
        }

        //Actualiza Token de Mediciación
        [HttpPut]
        public async Task<IActionResult> Put(int idMediacion, [FromBody]JArray documentos)
        {
            try
            {
                CreaAccountyGeth();

                await VerificaExistenciaSmartContract();
                
                var contract = geth.Eth.GetContract(abi, _smartContractConfiguration.SmartContractAddress);

                var agregaDocumentoFunction = contract.GetFunction("agregaDocumento");
                var mediacionStruct = contract.GetFunction("mediaciones");
                var seCreoNuevaDocumentoEvent = contract.GetEvent("SeCreoNuevoDocumento");

                foreach (JObject item in documentos)
                {
                    string descripcion = item.GetValue("descripcion").ToString();
                    string ipfsHash = item.GetValue("ipfsHash").ToString();

                    var transactionHash = await agregaDocumentoFunction.SendTransactionAsync(mediadorAddress, new HexBigInteger(900000), null, descripcion, Convert.ToInt32(idMediacion), ipfsHash);

                    var receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                    while (receipt == null){
                        Thread.Sleep(5000);
                        receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                    }
                }
                
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

        private void CreaAccountyGeth()
        {
            account = new ManagedAccount(mediadorAddress, password);  //Kaleido + Clique
            geth = new Web3Geth(account, "http://localhost:8501");  //Clique
            /**** Kaleido ****/
            /*var byteArray = Encoding.ASCII.GetBytes("u0n94z8hht:xt7u8OJu_88nAEvOeZ1wHqyT3Xz1a2cBuw0RRWQ3mL0");
            AuthenticationHeaderValue autenticacion = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var client = new RpcClient(new Uri("https://u0yaaa1f6n-u0s4p500nd-rpc.us-east-2.kaleido.io"),autenticacion);
            var geth = new Web3(client);*/
            /*****************/
        }

        private async Task VerificaExistenciaSmartContract() 
        {
            //if (contractAddress == string.Empty)
            if (_smartContractConfiguration.SmartContractDeployed == false)
            {
                try
                {
                    mediadorAddress = mediadorAddress.ToLower().RemoveHexPrefix().PadLeft(40,'0').EnsureHexPrefix();

                    var transactionHash = await geth.Eth.DeployContract.SendRequestAsync(abi, bytecode, mediadorAddress, new HexBigInteger(900000));
                    var receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                    while (receipt == null)
                    {
                        Thread.Sleep(5000);
                        receipt = await geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                    }

                    _smartContractConfiguration.SmartContractDeployed = true;
                    _smartContractConfiguration.SmartContractAddress = receipt.ContractAddress;
                }
                catch (Exception chingadamadre)
                {
                    Console.WriteLine("Message: " + chingadamadre.Message);
                    Console.WriteLine("InnerException: " + chingadamadre.InnerException);
                    Console.WriteLine("Source: " + chingadamadre.Source);
                    throw chingadamadre;
                }
            }
        }
    }

    [FunctionOutput]
    public class SeCreoNuevaMediacionEvent
    {
        [Parameter("address", "mediador", 1, true)]
        public string Mediador {get; set;}

        [Parameter("uint32", "idMediacion", 2, true)]
        public BigInteger IdMediacion {get; set;}

        [Parameter("string", "tipoDocto", 3, true)]
        public string TipoDocto {get; set;}

        [Parameter("string", "participante", 4, false)]
        public string Participante {get; set;}
    }
}