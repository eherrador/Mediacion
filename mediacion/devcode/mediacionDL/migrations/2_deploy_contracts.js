var Mediaciones = artifacts.require("Mediaciones");
var test = artifacts.require("test");

module.exports = function(deployer) {
  deployer.deploy(Mediaciones);
  deployer.deploy(test);
};