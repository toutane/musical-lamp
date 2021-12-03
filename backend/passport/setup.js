const User = require('../models/User')
const passport = require('passport');
const LocalStrategy = require('passport-local').Strategy;
const bcrypt = require('bcryptjs');

passport.serializeUser((user, done) => {
    done(null, user._id);
});

passport.deserializeUser((id, done) => {
    User.findById(id, (err, user) => {
        done(err, user);
    });
});

// Local Strategy
passport.use(
    new LocalStrategy(
        { usernameField: 'playername', passwordField: 'password' },
        (playername, password, done) => {
        // Match User
        User.findOne({ 'playername': playername}, function(err, user) {
            if (err) { return done(err); }
            if (!user) {
                return done(null, false, { message: 'Incorrect playername.' });
            } else {
            // Match Password
                bcrypt.compare(password, user.password, (err, isMatch) => {
                    if (err) {
                        return done(err, { message: 'Failure during password matching.' });
                    }
                    if (isMatch) {
                        return done(null, user);
                    } else {
                        return done(null, false, { message: 'Wrong password.' });
                    }
                })
            }
        })
    })
);

module.exports = passport;