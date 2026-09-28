const assert = require("node:assert/strict");
const test = require("node:test");
const { statement } = require("./statement");

const plays = {
  hamlet: { name: "Hamlet", type: "tragedy" },
  "as-like": { name: "As You Like It", type: "comedy" },
  othello: { name: "Othello", type: "tragedy" },
};

const invoice = {
  customer: "BigCo",
  performances: [
    { playID: "hamlet", audience: 55 },
    { playID: "as-like", audience: 35 },
    { playID: "othello", audience: 40 },
  ],
};

test("produces the existing customer statement", () => {
  const expected = [
    "Statement for BigCo",
    "  Hamlet: $650.00 (55 seats)",
    "  As You Like It: $580.00 (35 seats)",
    "  Othello: $500.00 (40 seats)",
    "Amount owed is $1,730.00",
    "You earned 47 credits",
    "",
  ].join("\n");

  assert.equal(statement(invoice, plays), expected);
});

test("rejects an unknown play type", () => {
  const experimentalPlays = {
    newPlay: { name: "A New Play", type: "experimental" },
  };
  const experimentalInvoice = {
    customer: "SmallCo",
    performances: [{ playID: "newPlay", audience: 10 }],
  };

  assert.throws(
    () => statement(experimentalInvoice, experimentalPlays),
    /unknown type: experimental/,
  );
});
