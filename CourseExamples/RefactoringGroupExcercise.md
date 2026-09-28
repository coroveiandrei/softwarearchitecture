# Refactoring group exercise

## Problem

A company of theatrical players performs plays at customer events. It calculates the price from the audience size and play type. Customers also earn volume credits that they can use for future discounts.

The current implementation supports tragedies and comedies. The billing calculation and plain-text formatting live in one `statement` function.

Starter implementation: [`statement.js`](RefactoringGroupExercise/statement.js)

The current output is:

```text
Statement for BigCo
  Hamlet: $650.00 (55 seats)
  As You Like It: $580.00 (35 seats)
  Othello: $500.00 (40 seats)
Amount owed is $1,730.00
You earned 47 credits
```

## Future changes

The product owner has requested two changes:

1. Produce an HTML statement containing the same customer, performances, total amount, and volume credits. The existing plain-text output must remain unchanged.
2. Support a new `pastoral` play type. A pastoral costs $40,000 plus $1,500 for each audience member above 20. It earns one volume credit for each audience member above 20.

Do not implement these requests during the exercise. Use them to decide which parts of the current function have different reasons to change.

## Assignment

1. List two or three reasons why the `statement` function may need to change.
2. Sketch the resulting functions, classes, or modules and explain how the design accommodates the two future changes.

## Time

30 minutes
