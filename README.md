# eilang
eilang is a scripting language. That's about it.

There's a basic Telegram bot implementation under Scripts/telegram.ei though.

Hello world:
```eilang
println('hello world');
# global expressions are run if there is no main function ("fun main()")
```

Arrays:
```eilang
fun main() {
    var primes = [3, 5, 7];
    for (primes) {
        println($"Prime number: {it} at index {ix}");
    }
    
    for ~(primes) {
        println($"Prime number in reverse: {it} at index {ix}");
    }
}
```

More complicated hello world:
```eilang
modu hello {
    typ world {
        ctor(name);

        fun greet() {
            println($'hello {name}!');
        }
    }
}

fun main() {
    *hello::world('human').greet(); # prints "hello human!"
    println($'ciao {*hello::world('human').name}'); # prints "ciao human"
}
```

More class functionality:
```eilang
typ point {
    ctor(x, y); # creates a constructor that sets the member variables 'x' and 'y'

    fun idx_get(idx) { # indexer, for no good reason
        if(idx == 0) {
            ret x;
        } else if (idx == 1) {
            ret y;
        }
        ret -1;
    }

    fun len() { # implementing idx_get(idx) and len() makes the class enumerable
        ret 2;
    }
}

fun main() {
    var p1 = *point(1,2); # constructing a point
    println("Using indexer to get x: " + p1[0]);
    println("Using indexer to get y: " + p1[1]);
    println($"Using member accessor to get x: {p1.x}");
    println($"Using member accessor to get y: {p1.y}");

    for(p1) {
        println($"Looping over point(1,2): {it}");
    }
}
```

Function references:
```eilang
typ point { ctor(x, y); }

fun add(p1, p2) {
    ret *point(p1.x + p2.x, p1.y + p2.y);
}

fun multiply(p1, p2) {
    ret *point(p1.x * p2.x, p1.y * p2.y);
}

fun main() {
    var p1 = *point(1, 2);
    var p2 = *point(3, 4);

    var math = @add; # references the function 'add'
    var added = math.call(p1, p2); # calls the referenced function
    println($"(1,2) + (3,4) = ({added.x},{added.y})"); # prints "(1,2) + (3,4) = (4,6)"

    math = @multiply;
    var multiplied = math.call(p1, p2);
    println($"(1,2) * (3,4) = ({multiplied.x},{multiplied.y})");
}
```