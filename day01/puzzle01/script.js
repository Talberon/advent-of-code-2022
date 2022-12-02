const fs = require('fs');

fs.readFile('./input.txt', (error, data) => {
  if (error) throw error;

  const input = data.toString();

  const elves = input.split('\r\n\r\n');

  const inventories = [];

  elves.forEach(inventory => inventories.push(
    inventory.split('\r\n')
      .map(calories => parseInt(calories))
      .reduce((total, calories) => total + calories)
  ));

  inventories.sort((a, b) => b - a);

  console.log("Max Calories: " + inventories[0]);

  let total = 0;

  for (i = 0; i < 3; i++) {
    console.log(`Top Three Calories #${i + 1} ` + inventories[i])
    total += inventories[i];
  }

  console.log("Top Three Sum: " + total)
})
