/**
 * Validates a date string in French format (DD/MM/YYYY) and converts it to a Date object
 * @param dateString - The date string to validate (e.g., "25/12/2023")
 * @returns Date object if valid, null if invalid
 */
export function validateFrenchDate(dateString: string): Date | null {
  // Check if input is a string and not empty
  if (!dateString || typeof dateString !== 'string') {
    return null;
  }

  // Regular expression for DD/MM/YYYY format
  const frenchDateRegex = /^([0-2]\d|3[01])\/(0[1-9]|1[0-2])\/(\d{4})$/;
  const match = dateString.trim().match(frenchDateRegex);
  
  if (!match) {
    return null;
  }

  const day = parseInt(match[1], 10);
  const month = parseInt(match[2], 10);
  const year = parseInt(match[3], 10);

  // Create date object (month is 0-based in JavaScript)
  const date = new Date(year, month - 1, day);

  // Validate that the date is actually valid
  // This catches cases like 31/02/2023 or 29/02/2021
  if (
    date.getFullYear() !== year ||
    date.getMonth() !== (month - 1) ||
    date.getDate() !== day
  ) {
    return null;
  }

  return date;
}

/**
 * Validates if a date string is in correct French format
 * @param dateString - The date string to validate
 * @returns boolean indicating if format is valid
 */
export function isValidFrenchDateFormat(dateString: string): boolean {
  return validateFrenchDate(dateString) !== null;
}

                  