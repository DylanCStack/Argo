var db = require('../db.js');

exports.register = function(phone, email, pass, done){
  var query = {"phone": phone};

  if (email !== null){query['email'] = email; console.log(query);}

  db.get().query('SELECT * FROM users WHERE ?;', {"phone": phone},  function(err, result){
    if(err) return done(err, false);
    // console.log(result);
    // console.log(result[0]);
    console.log(phone + " : " + pass + " : " + email);

    if(result[0] === undefined){
      query['password'] = pass;

      db.get().query("INSERT INTO users SET ?", query, function(error, rows){

        if(error){console.log("ERROR HERE " + error); return done(true, false);}

        console.log("User " + rows.insertId + " created.");

        return done(null, true);// registration successful
      })
    } else {
      return done(null, false);//registration not possible due to existing email pr phone
    }
  })
}
exports.login = function(phone, email, pass, done){
  var query = {"phone": phone};

  if (email !== null){query['email'] = email; console.log(query);}

  db.get().query("SELECT * FROM users WHERE ?;", {"phone": phone}, function(err, results){
    console.log("results");
    console.log(results);
    if(err) return done(err, null);//error in login

    if(results[0] !== undefined){
      return done(null, results[0]);//successful login
    } else {
      return done(null, null);//unsuccessful login
    }


  })
}
