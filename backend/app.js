var express = require('express');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');

var index = require('./routes/index');
var users = require('./routes/users');
var message = require('./routes/message');

var app = express();

// MySql Database setup
var db = require('./db');

// Connect to MySQL on start
db.connect(db.MODE_PRODUCTION, function(err) {
  if (err) {
    console.log('Unable to connect to MySQL.');
    process.exit(1)
  } else {
  //   app.listen(3000, function() {
      console.log('Database connected.');
      // console.log(db.get());
  //   })
  }
})
// session management setup
var session = require('client-sessions');
var secretKey = require('./secretKey');

app.use(session({
  cookieName: 'session',
  secret: secretKey,
  duration: 365 * 24 * 60 * 60 * 1000,//user will stay logged in for 1 year.
  activeDuration: 5 * 60 * 1000,
}));
var userController = require("./controllers/users");
app.use(function(req, res, next) {//checks if the user has a valid session
  // return res.send(req);
  // console.log(req);

  if (req.session && req.session.user) {//if session and user exist
    //in http request this checks key:'cookie':'session=***'
    console.log("SESSION EXISTS");
    userController.findByPhone({ phone: req.session.user.phone }, function(err, user) {
      if (user) {
        req.user = user;//check this to see if user is logged in
        delete req.user.password; // delete the password from the session
        req.session.user = user;  //refresh the session value
        res.locals.user = user;
      }
      // finishing processing the middleware and run the route
      next();
    });
  } else {
    next();
  }
});

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'pug');

// uncomment after placing your favicon in /public
//app.use(favicon(path.join(__dirname, 'public', 'favicon.ico')));
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

app.use('/', index);
app.use('/message', message);
app.use('/user', users);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  var err = new Error('Not Found');
  err.status = 404;
  next(err);
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render('error');
});

module.exports = app;
