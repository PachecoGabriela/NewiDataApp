function moveCursorToEndByClass(name) {
    const parentElement = document.querySelector('.' + name);
    if (parentElement) {
        const textarea = parentElement.querySelector('textarea');
        if (textarea) {
            textarea.focus();
            textarea.style.overflowY = "auto";
            textarea.style.flex = 1;
            textarea.style.paddingRight = "10px";

            requestAnimationFrame(() => {
                requestAnimationFrame(() => {
                    textarea.scrollTop = textarea.scrollHeight;


                    console.log('  textarea.scrollTop = textarea.scrollHeight; ' + textarea.scrollHeight);


                });
            });
        } else {
            console.log('Textarea not found within the element.');
        }
    } else {
        console.log('Element not found.');
    }
}


