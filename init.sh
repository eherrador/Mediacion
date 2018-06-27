#checando parametros de entrada
#The $# variable will tell you the number of input arguments the script was passed.
if [ $# -lt 2 ]
  then
    echo "Debes proporcionar 2 argumentos de entrada"
    echo "1er. argumento: El nombre del directorio raiz del proyecto"
    echo "2do. argumento: El password a usar para generar el coinbase"
    exit 1
fi

#crear los directorios
mkdir ./$1
cd ./$1
mkdir ./devnet
mkdir ./devcode
cd ./devnet
mkdir ./node1 ./node2
echo $1 >> ./networkname.txt
cp ../../startbootnode.sh .
cp ../../startnode1.sh .
cp ../../startnode2.sh .
cp ../../restartbootnode.sh .

#Para cada nodo se genera la llave pública y privada a partir de password que se pasa cómo segundo parametro
echo $2 >> ./password.txt
geth --datadir=./node1/ --password ./password.txt account new > ./node1/account1.txt;
geth --datadir=./node2/ --password ./password.txt account new > ./node2/account2.txt;
cat ./node1/account1.txt
cat ./node2/account2.txt

function pause(){
   read -p "$*"
}
pause 'Ahora vamos a crear la red Clique (PoA). Oprime cualquier tecla para continuar...'

#puppeth
puppeth  --network=$1

#/Users/edgarherrador/.puppeth/
