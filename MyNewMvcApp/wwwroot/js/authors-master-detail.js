(function () {
    // simple debounce
    function debounce(fn, wait) {
        let t = null;
        return function () {
            const args = arguments;
            clearTimeout(t);
            t = setTimeout(function () { fn.apply(null, args); }, wait);
        };
    }

    async function fetchHtml(url) {
        const res = await fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        if (!res.ok) throw new Error('Network error');
        return await res.text();
    }

    function init(el) {
        const input = document.getElementById('amd-search-input');
        const btn = document.getElementById('amd-search-btn');
        const master = document.getElementById('amd-master-placeholder');
        const detail = document.getElementById('amd-detail-placeholder');

        async function doSearch() {
            const q = input.value || '';
            try {
                master.innerHTML = 'Searching...';
                const html = await fetchHtml('/AuthorsMasterDetail/Search?name=' + encodeURIComponent(q));
                master.innerHTML = html;
                attachMasterHandlers();
            } catch (e) {
                master.innerHTML = '<div class="text-danger">Search failed</div>';
                console.error(e);
            }
        }

        const debouncedSearch = debounce(doSearch, 300);
        input.addEventListener('input', debouncedSearch);
        btn.addEventListener('click', doSearch);

        function attachMasterHandlers() {
            const items = document.querySelectorAll('.amd-author-item');
            items.forEach(it => {
                it.addEventListener('click', async function (ev) {
                    ev.preventDefault();
                    // remove active class
                    items.forEach(i => i.classList.remove('active'));
                    this.classList.add('active');
                    const id = this.getAttribute('data-id');
                    if (!id) return;
                    try {
                        detail.innerHTML = 'Loading...';
                        const html = await fetchHtml('/AuthorsMasterDetail/Books?id=' + encodeURIComponent(id));
                        detail.innerHTML = html;
                    } catch (e) {
                        detail.innerHTML = '<div class="text-danger">Failed to load books</div>';
                        console.error(e);
                    }
                });
            });
        }

        // If master already contains server rendered items, attach handlers
        attachMasterHandlers();
    }

    // Initialize when DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();
