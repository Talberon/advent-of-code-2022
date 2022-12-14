// Day 13: Distress Signal

// Int vs Int: If left side is higher, input is out of order
// [] vs []: If left side has more items, input is out of order. Else, iterate through ints. If left side is higher, input is out of order.
// [] vs Int: Compare int to the first item in []. If left side is higher, input is out of order.

const fs = require('fs');

fs.readFile('./input', (error, data) => {
  if (error) throw error;
  const input = data.toString().split(/\r?\n/).reverse();

  let pairIndex = 1;

  let runningSum = 0;

  while (input.length > 0) {

    const left = JSON.parse(input.pop());
    const right = JSON.parse(input.pop());

    if (input.length > 0) input.pop(); //Discard empty row

    console.log(`== Pair ${pairIndex} ==`);
    console.log(`Compare ${JSON.stringify(left)}\n     vs ${JSON.stringify(right)}\nSTART`);
    const isCorrect = isInCorrectOrder(left, right);
    if (isCorrect > 0) runningSum += pairIndex;
    console.log(`\n[Pair ${pairIndex}] Is in correct order?`, isCorrect, "Answer:", runningSum);

    pairIndex++;
    console.log("")
  }
});

function isInCorrectOrder(left, right) {
  for (let i = 0; i < left.length; i++) {
    console.log(`Compare ${JSON.stringify(left[i])} vs ${JSON.stringify(right[i])}`);

    if (right[i] == undefined) {
      console.log(`Right side ran out of items, so WRONG`);
      return -1;
    }

    if (typeof left[i] === 'number' && typeof right[i] == 'number') {
      if (left[i] < right[i]) {
        console.log(`Left side is smaller, so RIGHT`);
        return 1;
      }
      if (left[i] > right[i]) {
        console.log(`Right side is smaller, so WRONG`);
        return -1;
      }
      continue;
    }
    if (left[i] instanceof Array && right[i] instanceof Array) {
      const result = isInCorrectOrder(left[i], right[i]);
      if (result > 0) {
        console.log(`Propogating TRUE answer up`);
        return 1;
      }
      if (result < 0) {
        console.log(`Propogating FALSE answer up`);
        return -1;
      }
      console.log(`Continuing...`);
    }
    if (typeof left[i] == 'number' && right[i] instanceof Array) {
      console.log(`Mixed types; convert left to [${left[i]}] and retry comparison`);
      const result = isInCorrectOrder([left[i]], right[i]);
      if (result > 0) {
        console.log(`Propogating TRUE answer up`);
        return 1;
      }
      if (result < 0) {
        console.log(`Propogating FALSE answer up`);
        return -1;
      }
      console.log(`Continuing...`);
    }
    if (left[i] instanceof Array && typeof right[i] == 'number') {
      console.log(`Mixed types; convert right to [${right[i]}] and retry comparison`);
      const result = isInCorrectOrder(left[i], [right[i]]);
      if (result > 0) {
        console.log(`Propogating TRUE answer up`);
        return 1;
      }
      if (result < 0) {
        console.log(`Propogating FALSE answer up`);
        return -1;
      }
      console.log(`Continuing...`);
    }
  }

  if (left.length == right.length) {
    console.log(`Left and right are same size, so CONTINUE`);
    return 0;
  }

  if (left.length < right.length) {
    console.log(`Left side ran out of items, so RIGHT`);
    return 1;
  }
}