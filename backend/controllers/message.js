var db = require('../db.js');

exports.upload = function(url, privacy, recipient, done) {
  var values = [url, privacy, recipient];
  db.get().query('INSERT INTO messages (url, privacy, recipient) VALUES(?, ?, ?)', values, function(err, result) {
    if (err) return done(err)
    done(null, result.insertId)
  })
}

exports.getURL = function(messageID, done) {
  db.get().query('SELECT * FROM messages WHERE id = ?', messageID, function (err, rows) {
    if (err) return done(err)
    done(null, rows)
  })
}
