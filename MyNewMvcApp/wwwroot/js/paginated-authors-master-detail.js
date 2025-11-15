(function () {
    let currentPage = 1;
    let pageSize = 5;
    let currentSearchTerm = '';
    let totalPages = 0;

    const searchInput = document.getElementById('pamd-search-input');
    const searchBtn = document.getElementById('pamd-search-btn');
    const pageSizeSelect = document.getElementById('pamd-page-size');
    const masterPlaceholder = document.getElementById('pamd-master-placeholder');
    const detailPlaceholder = document.getElementById('pamd-detail-placeholder');
    const paginationDiv = document.getElementById('pamd-pagination');
    const pageInfo = document.getElementById('pamd-page-info');
    const paginationControls = document.getElementById('pamd-pagination-controls');

    // Search button click
    searchBtn.addEventListener('click', () => {
        currentSearchTerm = searchInput.value.trim();
        currentPage = 1;
        fetchAuthors();
    });

    // Enter key in search input
    searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            currentSearchTerm = searchInput.value.trim();
            currentPage = 1;
            fetchAuthors();
        }
    });

    // Page size change
    pageSizeSelect.addEventListener('change', (e) => {
        pageSize = parseInt(e.target.value);
        currentPage = 1;
        if (currentSearchTerm !== '' || masterPlaceholder.innerHTML !== 'Search to see authors...') {
            fetchAuthors();
        }
    });

    function fetchAuthors() {
        const url = `/api/Authors/paged?name=${encodeURIComponent(currentSearchTerm)}&pageNumber=${currentPage}&pageSize=${pageSize}`;
        console.log('Fetching:', url, 'JS pageSize:', pageSize);

        fetch(url, {
            cache: 'no-store',
            headers: {
                'Cache-Control': 'no-cache'
            }
        })
            .then(response => response.json())
            .then(data => {
                console.log('API Response:', data);
                totalPages = data.totalPages || 0;
                displayAuthors(data.items || []);
                updatePaginationControls(data);
            })
            .catch(error => {
                console.error('Error fetching authors:', error);
                masterPlaceholder.innerHTML = '<p class="text-danger">Error loading authors.</p>';
            });
    }

    function displayAuthors(authors) {
        if (authors.length === 0) {
            masterPlaceholder.innerHTML = '<p>No authors found.</p>';
            paginationDiv.style.display = 'none';
            return;
        }

        let html = '<div class="list-group">';
        authors.forEach(author => {
            html += `<a href="#" class="list-group-item list-group-item-action author-item" data-author-id="${author.id}">
                        <div class="d-flex w-100 justify-content-between">
                            <h5 class="mb-1">${escapeHtml(author.name || 'Unknown')}</h5>
                            <small>${escapeHtml(author.country || '')}</small>
                        </div>
                    </a>`;
        });
        html += '</div>';
        masterPlaceholder.innerHTML = html;

        // Add click handlers to author items
        document.querySelectorAll('.author-item').forEach(item => {
            item.addEventListener('click', (e) => {
                e.preventDefault();
                const authorId = item.getAttribute('data-author-id');
                loadAuthorDetails(authorId);

                // Highlight selected item
                document.querySelectorAll('.author-item').forEach(i => i.classList.remove('active'));
                item.classList.add('active');
            });
        });
    }

    function updatePaginationControls(data) {
        const totalCount = data.totalCount || 0;
        const pageNumber = data.pageNumber || 1;
        const totalPages = data.totalPages || 0;
        const apiPageSize = data.pageSize || pageSize;

        // Update page info (always, even if hiding pagination)
        const startItem = totalCount === 0 ? 0 : ((pageNumber - 1) * apiPageSize) + 1;
        const endItem = Math.min(pageNumber * apiPageSize, totalCount);
        pageInfo.textContent = `Showing ${startItem}-${endItem} of ${totalCount} authors`;

        if (totalPages <= 1) {
            paginationDiv.style.display = 'none';
            paginationControls.innerHTML = ''; // Clear old pagination controls
            return;
        }

        paginationDiv.style.display = 'flex';

        // Build pagination controls
        let paginationHtml = '';

        // Previous button
        paginationHtml += `<li class="page-item ${pageNumber === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${pageNumber - 1}">Previous</a>
        </li>`;

        // Page numbers
        const maxPagesToShow = 5;
        let startPage = Math.max(1, pageNumber - Math.floor(maxPagesToShow / 2));
        let endPage = Math.min(totalPages, startPage + maxPagesToShow - 1);

        if (endPage - startPage < maxPagesToShow - 1) {
            startPage = Math.max(1, endPage - maxPagesToShow + 1);
        }

        if (startPage > 1) {
            paginationHtml += `<li class="page-item"><a class="page-link" href="#" data-page="1">1</a></li>`;
            if (startPage > 2) {
                paginationHtml += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
            }
        }

        for (let i = startPage; i <= endPage; i++) {
            paginationHtml += `<li class="page-item ${i === pageNumber ? 'active' : ''}">
                <a class="page-link" href="#" data-page="${i}">${i}</a>
            </li>`;
        }

        if (endPage < totalPages) {
            if (endPage < totalPages - 1) {
                paginationHtml += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
            }
            paginationHtml += `<li class="page-item"><a class="page-link" href="#" data-page="${totalPages}">${totalPages}</a></li>`;
        }

        // Next button
        paginationHtml += `<li class="page-item ${pageNumber === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${pageNumber + 1}">Next</a>
        </li>`;

        paginationControls.innerHTML = paginationHtml;

        // Add click handlers to pagination links
        paginationControls.querySelectorAll('a.page-link').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const page = parseInt(link.getAttribute('data-page'));
                if (page && page !== currentPage && page >= 1 && page <= totalPages) {
                    currentPage = page;
                    fetchAuthors();
                }
            });
        });
    }

    function loadAuthorDetails(authorId) {
        fetch(`/api/Authors/${authorId}`)
            .then(response => response.json())
            .then(author => {
                let html = `<h4>${escapeHtml(author.name || 'Unknown')}</h4>`;
                html += `<p><strong>Country:</strong> ${escapeHtml(author.country || 'N/A')}</p>`;
                html += '<h5>Books:</h5>';
                
                if (author.books && author.books.length > 0) {
                    html += '<ul class="list-group">';
                    author.books.forEach(book => {
                        const title = book.originalTitle || book.englishTitle || 'Untitled';
                        html += `<li class="list-group-item">${escapeHtml(title)}</li>`;
                    });
                    html += '</ul>';
                } else {
                    html += '<p>No books found for this author.</p>';
                }

                detailPlaceholder.innerHTML = html;
            })
            .catch(error => {
                console.error('Error fetching author details:', error);
                detailPlaceholder.innerHTML = '<p class="text-danger">Error loading author details.</p>';
            });
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
})();
