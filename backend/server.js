const express = require('express');
const session = require('express-session');
const cookieParser = require("cookie-parser");
const MongoStore = require('connect-mongo');
const mongoose = require('mongoose');
// const cors = require("cors");

const passport = require('./passport/setup');

const auth = require('./routes/auth');
const player = require('./routes/player');

const app = express();
const keys = require('./config/keys.js');

// CORS options
// const corsOptions = {
//     credentials: true,
//     origin: [
//       'http://localhost:3000',
//       'http://192.168.86.171:3000',
//     ],
//   }

mongoose
    .connect(keys.mongoURI, { useNewUrlParser: true })
    .then(console.log(`MongoDB connected ${keys.mongoURI}`))
    .catch(err => console.log(err));

// Bodyparser middleware, extended false does not allow nested payloads
// app.use(cors(corsOptions));
app.use(cookieParser());
app.use(express.json());
app.use(express.urlencoded({ extended: false }));

// Express Session
app.use(
    session({
    secret: "foo",
    resave: false,
    saveUninitialized: true,
    cookie: { secure: false },
    store: MongoStore.create({
        mongoUrl: keys.mongoURI,
        collection: 'sessions'
    })
  })
);

// Passport middleware
app.use(passport.initialize());
app.use(passport.session());

// Routes
app.use('/api/auth', auth);
app.use('/api/player', player);

app.get('/', (req, res) => res.send("Good morning on musical-lamp backend server!"))

app.listen(keys.port, () => {
    console.log(`Server listenning on port ${keys.port}`);
})