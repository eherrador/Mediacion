module.exports = {
  networks: {
    development: {
      host: "127.0.0.1",
      port: 7545,
      network_id: "*" // Match any network id
    },
    mediacion_clique: {
      host: "127.0.0.1",
      port: 8501,
      network_id: "*" // Match network id
    }
  }
};