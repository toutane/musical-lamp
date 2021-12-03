const bcrypt = require('bcryptjs');
const express = require('express');
const auth = express.Router();
const passport = require('passport');
const User = require('../models/User');

// User Login
auth.post('/login', (req, res, next) => {
    passport.authenticate('local', function(err, user, info) {
        if (err) {
            return res.status(400).json({ errors: err });
        }
        if (!user) {
            return res.status(400).json({ errors: "User not found.", info: info.message });
        }
        req.logIn(user, function(err) {
            if (err) {
                return res.status(400).json({ errors: err });
            }
            
            return res.status(200).json({ success: `Logged in ${user.id}` });
        });
    })(req, res, next);
});

// User Registration
auth.post('/register', async (req, res, next) => {
    let user = await User.findOne({ 'playername': req.body.playername });
    if (user) {
        return res.status(400).send({ message: "Player already exists."})
    } else {
        const newUser = new User({ playername: req.body.playername, password: req.body.password });
        // Hash password before saving in database
        bcrypt.genSalt(10, (err, salt) => {
            if (err) {
                return res.status(400).json({ error: err, message: 'Failure during salt generation.'})
            }
            bcrypt.hash(newUser.password, salt, (err, hash) => {
                if (err) {
                    return res.status(400).json({ error: err, message: 'Failure during password hashing.'});
                }
                newUser.password = hash;
                newUser.save().then(user => {
                    console.log('Successful user registration.')
                    return res.status(200).json({ success: `User ${user.id} successfuly registred.`})
            })
        })
    })
    }
})

// User Logout
auth.get('/logout', (req, res) => {
    if (req.user) {
        let user_id = req.user.id;
        req.logout();
        res.status(200).send({ message: `User ${user_id} successfuly logout.`})        
    } else {
        res.status(400).send({ message: 'User is not logged-in, cannot logout.'})
    }
  });

module.exports = auth;