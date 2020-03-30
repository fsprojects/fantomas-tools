const nodeListToArray = nl => Array.prototype.slice.call(nl);
const dollar = selector => nodeListToArray(document.querySelectorAll(selector));

function scrollToImpl(idx, attempts) {
  if (attempts === 5) return;

  const details = dollar("#details .detail");
  const elem = details[idx];

  if (elem) {
    elem.scrollIntoView({
      behavior: "smooth",
      block: "end",
      inline: "center"
    });
    elem.classList.add("lit");
    setTimeout(() => {
      elem.classList.remove("lit");
    }, 1000);
  } else {
    setTimeout(() => {
      scrollToImpl(idx, attempts++); // nasty
    }, 150);
  }
}

export function scrollTo(idx) {
  scrollToImpl(idx, 0);
}