using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotNames
{
    /*
     * Thanks ChatGPT btw lol
     */
    private static readonly string[] names = new string[]
    {
        "Liam", "Noah", "Oliver", "James", "Elijah", "Mateo", "Theodore", "Henry", "Lucas", "William",
        "Benjamin", "Levi", "Sebastian", "Jack", "Ezra", "Michael", "Daniel", "Leo", "Owen", "Samuel",
        "Hudson", "Alexander", "Asher", "Luca", "Ethan", "John", "David", "Jackson", "Joseph", "Mason",
        "Luke", "Matthew", "Julian", "Dylan", "Elias", "Jacob", "Maverick", "Gabriel", "Logan", "Aiden",
        "Thomas", "Isaac", "Miles", "Grayson", "Santiago", "Anthony", "Wyatt", "Carter", "Jayden", "Ezekiel",
        "Caleb", "Cooper", "Josiah", "Charles", "Christopher", "Isaiah", "Nolan", "Cameron", "Nathan", "Joshua",
        "Kai", "Waylon", "Angel", "Lincoln", "Andrew", "Roman", "Adrian", "Aaron", "Wesley", "Ian",
        "Thiago", "Axel", "Brooks", "Bennett", "Weston", "Rowan", "Christian", "Theo", "Beau", "Eli",
        "Silas", "Jonathan", "Ryan", "Leonardo", "Walker", "Jaxon", "Micah", "Everett", "Robert", "Enzo",
        "Parker", "Jeremiah", "Jose", "Colton", "Luka", "Easton", "Landon", "Jordan", "Amir", "Gael",
        "Austin", "Adam", "Jameson", "August", "Xavier", "Myles", "Dominic", "Damian", "Nicholas", "Jace",
        "Carson", "Atlas", "Adriel", "Kayden", "Hunter", "River", "Greyson", "Emmett", "Harrison", "Vincent",
        "Milo", "Jasper", "Giovanni", "Jonah", "Zion", "Connor", "Sawyer", "Arthur", "Ryder", "Archer",
        "Lorenzo", "Declan", "Emiliano", "Luis", "Diego", "George", "Evan", "Jaxson", "Carlos", "Graham",
        "Juan", "Kingston", "Nathaniel", "Matteo", "Legend", "Malachi", "Jason", "Leon", "Dawson", "Bryson",
        "Amari", "Calvin", "Ivan", "Chase", "Cole", "Ashton", "Ace", "Arlo", "Dean", "Brayden",
        "Jude", "Hayden", "Max", "Matias", "Rhett", "Jayce", "Elliott", "Alan", "Braxton", "Kaiden",
        "Zachary", "Jesus", "Emmanuel", "Adonis", "Charlie", "Judah", "Tyler", "Elliot", "Antonio", "Emilio",
        "Camden", "Stetson", "Maxwell", "Ryker", "Justin", "Kevin", "Messiah", "Finn", "Bentley", "Ayden",
        "Zayden", "Felix", "Nicolas", "Miguel", "Maddox", "Beckett", "Tate", "Caden", "Beckham", "Andres",
        "Alejandro", "Alex", "Jesse", "Brody", "Tucker", "Jett", "Barrett", "Knox", "Hayes", "Peter",
        "Timothy", "Joel", "Edward", "Griffin", "Xander", "Oscar", "Victor", "Abraham", "Brandon", "Abel",
        "Richard", "Callum", "Riley", "Patrick", "Karter", "Malakai", "Eric", "Grant", "Israel", "Milan",
        "Gavin", "Rafael", "Tatum", "Kairo", "Elian", "Kyrie", "Louis", "Lukas", "Javier", "Nico",
        "Avery", "Rory", "Aziel", "Ismael", "Jeremy", "Zayn", "Cohen", "Simon", "Marcus", "Steven",
        "Mark", "Dallas", "Tristan", "Lane", "Blake", "Paul", "Paxton", "Bryce", "Nash", "Crew",
        "Kash", "Kenneth", "Omar", "Colt", "Lennox", "King", "Walter", "Emerson", "Phoenix", "Jaylen",
        "Derek", "Muhammad", "Ellis", "Kaleb", "Preston", "Jorge", "Zane", "Kayson", "Cade", "Tobias",
        "Otto", "Kaden", "Remington", "Atticus", "Finley", "Holden", "Jax", "Cash", "Martin", "Ronan",
        "Maximiliano", "Malcolm", "Romeo", "Josue", "Francisco", "Bodhi", "Cyrus", "Koa", "Angelo", "Aidan",
        "Jensen", "Erick", "Hendrix", "Warren", "Bryan", "Cody", "Leonel", "Onyx", "Ali", "Andre",
        "Jaziel", "Clayton", "Saint", "Dante", "Reid", "Casey", "Brian", "Gideon", "Niko", "Maximus",
        "Colter", "Kyler", "Brady", "Zyaire", "Cristian", "Cayden", "Harvey", "Cruz", "Dakota", "Damien",
        "Manuel", "Anderson", "Cairo", "Colin", "Joaquin", "Ezequiel", "Karson", "Callan", "Briggs", "Khalil",
        "Wade", "Jared", "Fernando", "Ari", "Colson", "Kylian", "Archie", "Banks", "Bowen", "Kade",
        "Daxton", "Jaden", "Rhys", "Sonny", "Zander", "Eduardo", "Iker", "Sullivan", "Bradley", "Raymond",
        "Odin", "Spencer", "Stephen", "Prince", "Brantley", "Killian", "Kamari", "Cesar", "Dariel", "Eithan",
        "Mathias", "Ricardo", "Orion", "Titus", "Luciano", "Rylan", "Pablo", "Chance", "Travis", "Kohen",
        "Marco", "Jay", "Malik", "Hector", "Edwin", "Armani", "Bodie", "Shiloh", "Marshall", "Remy",
        "Russell", "Baylor", "Kameron", "Tyson", "Grady", "Oakley", "Baker", "Winston", "Kane", "Julius",
        "Desmond", "Royal", "Sterling", "Mario", "Kylo", "Sergio", "Jake", "Kashton", "Shepherd", "Franklin",
        "Ibrahim", "Ares", "Koda", "Lawson", "Hugo", "Kyle", "Kyson", "Kobe", "Pedro", "Santino",
        "Wilder", "Raiden", "Damon", "Nasir", "Sean", "Forrest", "Kian", "Reed", "Tanner", "Jalen",
        "Apollo", "Zayne", "Nehemiah", "Edgar", "Johnny", "Clark", "Eden", "Gunner", "Isaias", "Esteban",
        "Hank", "Alijah", "Solomon", "Wells", "Sutton", "Royce", "Callen", "Reece", "Gianni", "Noel",
        "Quinn", "Raphael", "Corbin", "Erik", "Tripp", "Atreus", "Francis", "Kayce", "Callahan", "Devin",
        "Troy", "Sylas", "Fabian", "Zaire", "Donovan", "Johnathan", "Frank", "Lewis", "Moshe", "Adan",
        "Alexis", "Tadeo", "Ronin", "Marcos", "Kieran", "Leonidas", "Bo", "Kendrick", "Ruben", "Camilo",
        "Garrett", "Matthias", "Emanuel", "Jeffrey", "Collin", "Lucian", "Augustus", "Memphis", "Rowen"
    };

    public static string GetRandomBotName()
    {
        return names[Random.Range(0, names.Length)];
    }
}
