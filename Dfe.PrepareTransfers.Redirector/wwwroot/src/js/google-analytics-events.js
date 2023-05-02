const statusAttributeName = "data-google-analytics-project-status";
const generateButtonAttributeName = "data-google-analytics-project-generate";

function initialise() {
  const eventBody = { 'event': "project-template-downloaded" };

  const statuses = document.querySelectorAll(`[${statusAttributeName}]`);

  statuses.forEach((status) => {
    const attributeValue = status.getAttribute(statusAttributeName);
    const [statusName, completed] = attributeValue.split(",");
    // Easiest way to convert string to Boolean in JS.
    eventBody[statusName] = completed === "true";
  });

  const generateButton = document.querySelectorAll(
    `[${generateButtonAttributeName}]`
  )[0];

  // Using the click event because the button performs a GET request
  generateButton.addEventListener("click", () => {
    const dataLayer = window.dataLayer = window.dataLayer || [];
    dataLayer.push(eventBody);
  });
}

initialise();
