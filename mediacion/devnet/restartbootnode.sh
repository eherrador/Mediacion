rm -rf ./node1/geth
rm -rf ./node2/geth

geth --datadir ./node1/ init genesis.json
geth --datadir ./node2/ init genesis.json

bootnode -nodekey ./boot.key -verbosity 9 -addr :30310 self=enode://2b35ac907f8a9be8ec34180644bb906731df44525069c4c289ce8bdf0fe721e42ec922fc78d641b158814f0cd3abad76c766ba74dbf4a52df5959ceaf52da12c@127.0.0.1:30310
