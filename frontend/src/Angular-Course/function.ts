function Add6(num1: number, num2: number, ...num3 : number[]): number{
     return num1 + num2 + num3.reduce((a,b) => a*b,1);
}

let numbers = [1,1,1,1,3]
console.log(Add6(2,2, ...numbers));






// Three types of function :
// Name Function
function Add1 (num1 : number , num2 : number) : number {
    return num1+num2;
}
console.log(Add1(1,1));


// Arrow Function
const Add2 = (num1: number, num2: number): number => num1+ num2;
console.log(Add2(1,1));


// Function Expression
const Add3 = function(num1: number, num2: number): number{
    return num1+num2;
}
console.log(Add3(1,1));


// Optional Parameter : An Optional Parameter is a function method input that does not require
// argument to be supplied when the function is called. A parameter marked with `?` is optional, 
// so an argument does not have to be provided when calling the function.

// num3?: number
// → num3 can be a number OR undefined if omitted.
 
// Rules:
// 1. Use `?` after the parameter name.
// 2. Optional parameters can be omitted.
// 3. If omitted, the value is `undefined`.
// 4. Optional parameters must come after required parameters.

function Add4(num1: number, num2: number, num3?: number): number{
    return num3? num1+num2+num3: num1+num2;
}
console.log(Add4(1,1)) // num3 omitted → 2
console.log(Add4(1,1,1))  // num3 = 1 → 3



// Required parameter: A required parameter is an input field or argument that must be provided 
// when calling a function, method, API or command. 
// A default parameter is a parameter that is automatically assigned 
// a default value when no value is provided when calling a function.


const Add5 = (num1: number, num2: number, num3 =5) : number => num1+ num2 + num3;

//num1 → Required parameter 
//num2 → Required parameter 
//num3 = 5 → Default parameter (optional when calling) 

// If num3 is provided, the provided value is used.
console.log(Add5(0,0,6));

// If num3 is not provided, the default value (5) is used.
console.log(Add5(0,0));


