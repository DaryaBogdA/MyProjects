document.addEventListener('DOMContentLoaded', async () => {
    const uid = localStorage.getItem('userId');
    if (!uid) {
        window.location.href = '/login.html';
        return;
    }

    const esc =
        typeof escapeHtml === 'function'
            ? escapeHtml
            : function (s) {
                if (!s) return '';
                return String(s)
                    .replace(/&/g, '&amp;')
                    .replace(/</g, '&lt;')
                    .replace(/>/g, '&gt;')
                    .replace(/"/g, '&quot;')
                    .replace(/'/g, '&#39;');
            };

    function messagesHref(conversationId) {
        const path = window.location.pathname || '/messages.html';
        const q = new URLSearchParams(window.location.search);
        if (conversationId) q.set('conversation', String(conversationId));
        else q.delete('conversation');
        const s = q.toString();
        return s ? `${path}?${s}` : path;
    }

    function replaceMessagesHistory(conversationId) {
        const next = messagesHref(conversationId || undefined);
        if (window.location.pathname + window.location.search !== next) {
            history.replaceState(null, '', next);
        }
    }

    const convList = document.getElementById('convList');
    const chatHeader = document.getElementById('chatHeader');
    const chatMessages = document.getElementById('chatMessages');
    const openListingBtn = document.getElementById('openListingBtn');
    let currentConversationId = String(new URLSearchParams(window.location.search).get('conversation') || '').trim();
    let currentListingId = null;

    function setOpenListingEnabled(on, listingId) {
        if (!openListingBtn) return;
        if (!on || !listingId) {
            openListingBtn.href = '#';
            openListingBtn.style.pointerEvents = 'none';
            openListingBtn.style.opacity = '.65';
            return;
        }
        openListingBtn.href = `/listing-details.html?id=${listingId}`;
        openListingBtn.style.pointerEvents = 'auto';
        openListingBtn.style.opacity = '1';
    }

    function showNoDialogsYet() {
        chatHeader.textContent = 'Нет диалогов';
        chatMessages.innerHTML =
            '<div style="padding:16px;"><p style="margin:0 0 10px;color:var(--text-color);">' +
            'У вас пока нет диалогов.</p><p style="margin:0;font-size:0.92rem;color:var(--text-muted);">' +
            'Откройте объявление на сайте и нажмите «Открыть чат с собственником», чтобы начать переписку.</p></div>';
        setOpenListingEnabled(false);
        removeMessageInput();
    }

    function clearConversationSelection() {
        currentConversationId = '';
        currentListingId = null;
        replaceMessagesHistory('');
        chatHeader.textContent = 'Выберите диалог';
        chatMessages.innerHTML =
            '<p style="color:var(--text-muted); padding:16px;">Выберите диалог в списке слева.</p>';
        setOpenListingEnabled(false);
        removeMessageInput();
    }

    function createMessageInput() {
        removeMessageInput();

        if (!currentConversationId) return;

        const container = document.createElement('div');
        container.id = 'messageInputContainer';
        container.style.cssText = 'display:flex; gap:8px; padding:12px; border-top:1px solid var(--border-color); background:white;';
        container.innerHTML = `
            <input type="text" id="messageInput" placeholder="Введите сообщение..." style="flex:1; padding:10px 14px; border:1px solid var(--border-color); border-radius:24px; font-size:0.95rem; outline:none; transition:all 0.2s;">
            <button id="sendMessageBtn" class="btn btn-primary" style="border-radius:24px; padding:10px 20px;">
                 Отправить
            </button>
        `;

        const chatSection = document.querySelector('section');
        if (chatSection && !chatSection.querySelector('#messageInputContainer')) {
            chatSection.appendChild(container);

            const messageInput = document.getElementById('messageInput');
            const sendBtn = document.getElementById('sendMessageBtn');

            if (messageInput) {
                messageInput.addEventListener('focus', () => {
                    messageInput.style.borderColor = 'var(--primary-teal)';
                });
                messageInput.addEventListener('blur', () => {
                    messageInput.style.borderColor = 'var(--border-color)';
                });
                messageInput.addEventListener('keypress', (e) => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        sendBtn?.click();
                    }
                });
            }

            if (sendBtn) {
                const newSendBtn = sendBtn.cloneNode(true);
                sendBtn.parentNode.replaceChild(newSendBtn, sendBtn);

                newSendBtn.addEventListener('click', async () => {
                    await sendMessage();
                });
            }
        }
    }

    function removeMessageInput() {
        const container = document.getElementById('messageInputContainer');
        if (container) container.remove();
    }

    async function sendMessage() {
        const messageInput = document.getElementById('messageInput');

        if (!messageInput) {
            console.log('Message input not found');
            return;
        }

        const message = messageInput.value.trim();

        console.log('Message value:', message);

        if (!message) {
            console.log('Message is empty');
            return;
        }

        if (!currentConversationId) {
            console.log('No conversation selected');
            return;
        }

        const sendBtn = document.getElementById('sendMessageBtn');
        const originalBtnHtml = sendBtn ? sendBtn.innerHTML : '';

        if (sendBtn) {
            sendBtn.disabled = true;
            sendBtn.innerHTML = '<i class="fas fa-spinner fa-pulse"></i>';
        }

        try {
            const resp = await fetch(`/api/messages/${currentConversationId}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-User-ID': uid },
                body: JSON.stringify({ message }),
            });
            const data = await resp.json().catch(() => ({}));
            if (!resp.ok) throw new Error(data.error || 'Не удалось отправить сообщение');

            messageInput.value = '';
            await loadMessages();
        } catch (e) {
            console.error('Send message error:', e);
        } finally {
            if (sendBtn) {
                sendBtn.disabled = false;
                sendBtn.innerHTML = originalBtnHtml;
            }
        }
    }

    async function loadConversations() {
        try {
            const res = await fetch('/api/messages/conversations', { headers: { 'X-User-ID': uid } });
            const data = await res.json();
            if (!res.ok) throw new Error(data.error || 'Не удалось загрузить диалоги');
            convList.innerHTML = '';
            if (!Array.isArray(data) || data.length === 0) {
                convList.innerHTML =
                    '<p style="padding:14px;line-height:1.45;color:var(--text-muted);">У вас пока нет диалогов.</p>';
                currentConversationId = '';
                currentListingId = null;
                replaceMessagesHistory('');
                showNoDialogsYet();
                return;
            }

            const idSet = new Set(data.map((c) => String(c.id)));

            data.forEach((c) => {
                const cid = String(c.id);
                const item = document.createElement('button');
                item.type = 'button';
                item.dataset.conversationId = cid;
                item.dataset.listingId = c.listing_id;
                const selected = currentConversationId && cid === currentConversationId;
                item.style.cssText =
                    'display:block;width:100%;text-align:left;padding:10px 12px;border:none;border-bottom:1px solid var(--border-color);cursor:pointer;' +
                    (selected ? 'background:#eef8f6;' : 'background:white;');
                item.innerHTML = `<div style="font-weight:600;">${esc(c.listing_title || ('Объявление #' + c.listing_id))}</div><div style="font-size:0.85rem;color:var(--text-muted);">${esc(c.last_message || 'Без сообщений')}</div>`;
                item.addEventListener('click', () => {
                    currentConversationId = cid;
                    currentListingId = c.listing_id;
                    convList.querySelectorAll('button[data-conversation-id]').forEach((b) => {
                        b.style.background = b.dataset.conversationId === cid ? '#eef8f6' : 'white';
                    });
                    replaceMessagesHistory(cid);
                    setOpenListingEnabled(true, c.listing_id);
                    loadMessages();
                    createMessageInput();
                });
                convList.appendChild(item);
            });

            if (!currentConversationId || !idSet.has(currentConversationId)) {
                if (currentConversationId && !idSet.has(currentConversationId)) {
                    replaceMessagesHistory('');
                }
                clearConversationSelection();
                return;
            }

            const selected = data.find((c) => String(c.id) === String(currentConversationId));
            setOpenListingEnabled(!!selected, selected?.listing_id);
            currentListingId = selected?.listing_id;
            createMessageInput();
        } catch (e) {
            console.error('Load conversations error:', e);
            chatMessages.innerHTML = `<p class="error">${esc(e.message || 'Ошибка загрузки диалогов')}</p>`;
        }
    }

    async function loadMessages() {
        if (!currentConversationId) {
            chatMessages.innerHTML = '<p style="color:var(--text-muted); padding:16px;">Выберите диалог для просмотра сообщений.</p>';
            return;
        }

        try {
            const res = await fetch(`/api/messages/${currentConversationId}`, { headers: { 'X-User-ID': uid } });
            const data = await res.json();
            if (!res.ok) throw new Error(data.error || 'Не удалось загрузить сообщения');

            chatHeader.textContent = `Диалог #${currentConversationId}`;
            chatMessages.innerHTML = '';

            if (!Array.isArray(data) || data.length === 0) {
                chatMessages.innerHTML = '<p style="color:var(--text-muted); text-align:center; padding:40px;">Напишите первое сообщение!</p>';
                return;
            }

            data.forEach((m) => {
                const mine = String(m.sender_id) === String(uid);
                const row = document.createElement('div');
                row.style.cssText = `margin-bottom:12px; display:flex; ${mine ? 'justify-content:flex-end;' : ''}`;
                row.innerHTML = `
                    <div style="max-width:75%;">
                        <div style="background:${mine ? 'var(--primary-teal)' : '#f3f4f6'}; color:${mine ? 'white' : 'var(--text-dark)'}; padding:10px 14px; border-radius:${mine ? '18px 18px 4px 18px' : '18px 18px 18px 4px'};">
                            <div style="font-size:0.85rem; margin-bottom:4px; ${mine ? 'opacity:0.8' : 'color:var(--text-muted)'}">${esc(m.sender_name || '')}</div>
                            <div>${esc(m.body || '')}</div>
                        </div>
                        <div style="font-size:0.7rem; color:var(--text-muted); margin-top:4px; ${mine ? 'text-align:right' : 'text-align:left'}">
                            ${m.created_at ? new Date(m.created_at).toLocaleTimeString([], {hour:'2-digit', minute:'2-digit'}) : ''}
                        </div>
                    </div>
                `;
                chatMessages.appendChild(row);
            });
            chatMessages.scrollTop = chatMessages.scrollHeight;
        } catch (e) {
            console.error('Load messages error:', e);
            chatMessages.innerHTML = `<p class="error">${esc(e.message || 'Ошибка загрузки сообщений')}</p>`;
        }
    }

    try {
        await loadConversations();
        if (currentConversationId) {
            await loadMessages();
            createMessageInput();
        }
    } catch (e) {
        console.error('Initialization error:', e);
        chatMessages.innerHTML = `<p class="error">${esc(e.message || 'Ошибка')}</p>`;
    }
});