#checando parametros de entrada
#The $# variable will tell you the number of input arguments the script was passed.
if [ $# -lt 2 ]
  then
    echo "Debes proporcionar 2 argumentos de entrada"
    echo "1er. argumento: El NetworkId y debe ser el mismo que se asigno en el puppeth"
    echo "2do. argumento: La dirección de la cuenta en hexadecimal, por ejemplo 0xcdf8b126b9029a044f16ce680a686fee0b50a811 y debe ser la misma que se genero al ejecutar inint.sh y que se asigno en el puppeth"
    cat ./node1/account1.txt
    exit 1
fi

geth --datadir ./node1/ --syncmode 'full' --port 30311 --rpc --rpcaddr 'localhost' --rpcport 8501 --rpcapi 'personal,db,eth,net,web3,txpool,miner,admin,debug' --bootnodes 'enode://2b35ac907f8a9be8ec34180644bb906731df44525069c4c289ce8bdf0fe721e42ec922fc78d641b158814f0cd3abad76c766ba74dbf4a52df5959ceaf52da12c@127.0.0.1:30310' --networkid $1 --targetgaslimit 94000000 --gasprice '1' --unlock $2 --password ./password.txt --mine