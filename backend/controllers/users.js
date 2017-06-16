var db = require('../db.js');

exports.register = function(name, pass, done){
  db.get().query('SELECT * FROM users WHERE username = ?;',[name],  function(err, result){
    if(err) return done(err, false);
    // console.log(result);
    // console.log(result[0]);
    console.log(name + " : " + pass);

    if(result[0] === undefined){
      db.get().query("INSERT INTO users SET ?", {"username" :name, "password":pass}, function(error, rows){
        console.log("ERROR HERE " + error);
        if(error) return done(true, false);

        return done(null, true);// registration successful
      })
    } else {
      return done(null, false);//registration not possible

    }
  })
}
