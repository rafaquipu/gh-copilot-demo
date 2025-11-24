import {describe, it} from 'mocha';
import {expect} from 'chai';
import { validateFrenchDate } from './validators';  

// test the validateFrenchDate function
describe('validateFrenchDate', () => {
  it('should return a Date object for valid date strings', () => {
    const validDates = [
      { input: '25/12/2023', expected: new Date(2023, 11, 25) },
      { input: '01/01/2000', expected: new Date(2000, 0, 1) },
      { input: '29/02/2020', expected: new Date(2020, 1, 29) }, // leap year
    ];
    validDates.forEach(({ input, expected }) => {
      const result = validateFrenchDate(input);
      expect(result).to.deep.equal(expected);
    });
    it('should return null for invalid date strings', () => {
      const invalidDates = [
        '31/02/2023', // invalid day for February
        '29/02/2021', // not a leap year
        '32/01/2023', // invalid day
        '15/13/2023', // invalid month
        '15-12-2023', // wrong format
        '2023/12/15', // wrong format
        '',           // empty string
        null as any, // null input
        undefined as any, // undefined input
        12345 as any, // non-string input
      ]; 
      invalidDates.forEach(input => {
        const result = validateFrenchDate(input);
        expect(result).to.be.null;
      });
      it('should trim whitespace and validate correctly', () => {
        const result = validateFrenchDate('  05/11/2022  ');
        expect(result).to.deep.equal(new Date(2022, 10, 5));
      });
    });
  });
});