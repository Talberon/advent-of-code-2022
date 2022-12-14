// Day 13: Distress Signal
const fs = require('fs');

fs.readFile('./input', (error, data) => {
  if (error) throw error;
  const input = data.toString().split(/\r?\n/).reverse();

  const answer01 = Part01(input);
  console.log(`Part 1: `, answer01);

  const dividerPackets = ["[[2]]", "[[6]]"];
  const answer02 = Part02(input, dividerPackets);
  console.log(`Part 2: `, answer02);
});

function Part02(input, dividerPackets) {
  const allPackets = [...input, ...dividerPackets]
    .filter(line => line.length > 0)
    .map(line => JSON.parse(line))
    .sort((a, b) => isInCorrectOrder(b, a));

  let dividerPacketProduct = 1;

  for (let i = 0; i < allPackets.length; i++) {
    const current = JSON.stringify(allPackets[i]);
    console.log(`[${i + 1}]\t${current}`);
    if (dividerPackets.includes(current)) {
      dividerPacketProduct *= (i + 1);
    }
  }

  return dividerPacketProduct;
}

function Part01(input) {
  const lines = [...input];

  let pairIndex = 1;
  let runningSum = 0;

  while (lines.length > 0) {

    const left = JSON.parse(lines.pop());
    const right = JSON.parse(lines.pop());
    if (lines.length > 0) lines.pop(); //Discard empty row

    const isCorrect = isInCorrectOrder(left, right);
    if (isCorrect > 0)
      runningSum += pairIndex;

    pairIndex++;
  }

  return runningSum;
}

function isInCorrectOrder(left, right) {
  for (let i = 0; i < left.length; i++) {

    if (right[i] == undefined) return -1;

    if (typeof left[i] === 'number' && typeof right[i] == 'number') {
      if (left[i] < right[i]) return 1;
      if (left[i] > right[i]) return -1;
      continue;
    }
    if (left[i] instanceof Array && right[i] instanceof Array) {
      const result = isInCorrectOrder(left[i], right[i]);
      if (result > 0) return 1;
      if (result < 0) return -1;
    }
    if (typeof left[i] == 'number' && right[i] instanceof Array) {
      const result = isInCorrectOrder([left[i]], right[i]);
      if (result > 0) return 1;
      if (result < 0) return -1;
    }
    if (left[i] instanceof Array && typeof right[i] == 'number') {
      const result = isInCorrectOrder(left[i], [right[i]]);
      if (result > 0) return 1;
      if (result < 0) return -1;
    }
  }
  
  if (left.length == right.length) return 0;

  if (left.length < right.length) return 1;
}